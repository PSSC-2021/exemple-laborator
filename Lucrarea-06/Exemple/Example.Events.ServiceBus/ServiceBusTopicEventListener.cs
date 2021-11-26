using Azure.Messaging.ServiceBus;
using CloudNative.CloudEvents.SystemTextJson;
using Example.Events.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Events.ServiceBus
{
    public class ServiceBusTopicEventListener: IEventListener
    {
        private const string TopicName = "grades";
        private const string SubscriptionName = "All";
        private ServiceBusProcessor? processor;
        private readonly ServiceBusClient client;
        private readonly Dictionary<string, IEventHandler> eventHandlers;
        private readonly ILogger<ServiceBusTopicEventListener> logger;
        private readonly JsonEventFormatter formatter = new();

        public ServiceBusTopicEventListener(ServiceBusClient client, ILogger<ServiceBusTopicEventListener> logger, IEnumerable<IEventHandler> eventHandlers)
        {
            this.client = client;
            this.eventHandlers = eventHandlers.SelectMany(handler => handler.EventTypes
                                                                            .Select(eventType => (eventType, handler)))
                                                                            .ToDictionary(pair => pair.eventType, pair => pair.handler);
            this.logger = logger;
        }

        public Task StartAsync(string topicName, string subscriptionName, CancellationToken cancellationToken)
        {
            var options = new ServiceBusProcessorOptions
            {
                // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
                // Set AutoCompleteMessages to false to [settle messages](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
                // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
                AutoCompleteMessages = false,

                // I can also allow for multi-threading
                MaxConcurrentCalls = 2
            };
            processor = client.CreateProcessor(topicName, subscriptionName, options);
            processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
            return processor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await processor!.StopProcessingAsync(cancellationToken);
            processor.ProcessMessageAsync -= Processor_ProcessMessageAsync;
            processor.ProcessErrorAsync -= Processor_ProcessErrorAsync;
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            logger.LogError(arg.Exception, $"{arg.ErrorSource}, {arg.FullyQualifiedNamespace}, {arg.EntityPath}");
            return Task.CompletedTask;
        }

        private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            if (await EnsureMaxDeliveryCountAsync(arg))
            {
                await ProcessMessageAsCloudEventAsync(arg);
            }
        }

        private async Task<bool> EnsureMaxDeliveryCountAsync(ProcessMessageEventArgs arg)
        {
            bool canContinue = true;
            if (arg.Message.DeliveryCount > 5)
            {
                logger.LogError($"Retry count exceeded {arg.Message.MessageId}");
                await arg.DeadLetterMessageAsync(arg.Message, "Retry count exeeded");
                canContinue = false;
            }
            return canContinue;
        }

        private async Task ProcessMessageAsCloudEventAsync(ProcessMessageEventArgs arg)
        {
            var data = arg.Message.Body;
            var cloudEvent = formatter.DecodeStructuredModeMessage(data.ToStream(), null, null);
            if (eventHandlers.TryGetValue(cloudEvent.Type!, out var handler))
            {
                var result = await InvokeHandlerAsync(cloudEvent, handler);
                await InterpretResult(result, arg);
            }
            else
            {
                logger.LogError($"No handler found for {cloudEvent.Type}");
            }
        }

        private async Task<EventProcessingResult> InvokeHandlerAsync(CloudNative.CloudEvents.CloudEvent cloudEvent, IEventHandler handler)
        {
            try
            {
                return await handler.HandleAsync(cloudEvent);
            }
            catch (Exception ex)
            {
                //unexpected error
                logger.LogError(ex, ex.Message);
                return EventProcessingResult.Failed;
            }
        }

        private Task InterpretResult(EventProcessingResult result, ProcessMessageEventArgs arg) => result switch
        {
            EventProcessingResult.Completed => HandleProcessSuccessAsync(arg),
            EventProcessingResult.Retry => HandleProcessRetryAsync(arg),
            _ => HandleProcessErrorAsync(arg)
        };

        private Task HandleProcessErrorAsync(ProcessMessageEventArgs arg)
        {
            logger.LogError($"Event processing has failed {arg.Message.MessageId}");
            return arg.DeadLetterMessageAsync(arg.Message, "Processing of event has failed");
        }

        private Task HandleProcessRetryAsync(ProcessMessageEventArgs arg)
        {
            logger.LogWarning($"Event processing indicated retry {arg.Message.MessageId}");
            return arg.AbandonMessageAsync(arg.Message);
        }

        private Task HandleProcessSuccessAsync(ProcessMessageEventArgs arg)
        {
            logger.LogInformation($"Event processing has succedded {arg.Message.MessageId}");
            return arg.CompleteMessageAsync(arg.Message);
        }
    }
}
