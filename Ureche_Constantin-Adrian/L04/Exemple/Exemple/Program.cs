using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using static Exemple.Domain.Models.Product;
using static Exemple.Domain.ProductOperation;
using Exemple.Domain;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Net.Http;
using Example.Data.Repositories;
using Example.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exemple
{
    class Program
    {
        private static readonly Random random = new Random();

        private static string ConnectionString = "Server=LAPTOP-5O6G7HEC\\DEVELOPER;Database=PSSC-sample;Trusted_Connection=True;MultipleActiveResultSets=true";

        static async Task Main(string[] args)
        {
            using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
            ILogger<PublishProductWorkflow> logger = loggerFactory.CreateLogger<PublishProductWorkflow>();

            var listOfProduct = ReadListOfProduct().ToArray();
            PublishProductCommand command = new(listOfProduct);
            var dbContextBuilder = new DbContextOptionsBuilder<ProductContext>()
                                                .UseSqlServer(ConnectionString)
                                                .UseLoggerFactory(loggerFactory);
            ProductContext productContext = new ProductContext(dbContextBuilder.Options);
            CartRepository cartRepository = new(productContext);
            ProductRepository productRepository = new(productContext);
            PublishProductWorkflow workflow = new(cartRepository, productRepository, logger);
            var result = await workflow.ExecuteAsync(command);

            result.Match(
                    whenProductPublishFaildEvent: @event =>
                    {
                        Console.WriteLine($"Publish failed: {@event.Reason}");
                        return @event;
                    },
                    whenProductPublishScucceededEvent: @event =>
                    {
                        Console.WriteLine($"Publish succeeded.");
                        Console.WriteLine(@event.Csv);
                        return @event;
                    }
                );
        }

        private static ILoggerFactory ConfigureLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
                                builder.AddSimpleConsole(options =>
                                {
                                    options.IncludeScopes = true;
                                    options.SingleLine = true;
                                    options.TimestampFormat = "hh:mm:ss ";
                                })
                                .AddProvider(new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()));
        }

        private static List<UnvalidatedCart> ReadListOfProduct()
        {
            List<UnvalidatedCart> listOfProduct = new();
            do
            {
                //read registration number and grade and create a list of greads
                var cod = ReadValue("Cod: ");
                if (string.IsNullOrEmpty(cod))
                {
                    break;
                }

                var price = ReadValue("Price: ");
                if (string.IsNullOrEmpty(price))
                {
                    break;
                }

                var cantity = ReadValue("Cantity: ");
                if (string.IsNullOrEmpty(cantity))
                {
                    break;
                }

                listOfProduct.Add(new(cod, price, cantity));
            } while (true);
            return listOfProduct;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}
