using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Example.Events.Models;

namespace Example.Events
{
    public abstract class AbstractEventHandler<T> : IEventHandler where T : notnull
    {
        public abstract string[] EventTypes { get; }

        public Task<EventProcessingResult> HandleAsync(CloudEvent cloudEvent)
        {
            T eventData = DeserializeEvent(cloudEvent);
            return OnHandleAsync(eventData);
        }

        protected abstract Task<EventProcessingResult> OnHandleAsync(T eventData);

        private T DeserializeEvent(CloudEvent cloudEvent)
        {
            if (cloudEvent.Data is not null)
            {
                var json = ((JsonElement)cloudEvent.Data).GetRawText();
                var input = JsonSerializer.Deserialize<T>(json);
                if (input is not null)
                {
                    return input;
                }
                throw new NullReferenceException($"Deserializing event generated null value. {json}");
            }
            throw new NullReferenceException("CloudEvent Data cannot be null");
        }
    }
}
