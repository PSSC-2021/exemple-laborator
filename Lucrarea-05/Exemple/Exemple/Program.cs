using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using static Exemple.Domain.Models.ExamGrades;
using static Exemple.Domain.ExamGradesOperation;
using Exemple.Domain;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Net.Http;
using Example.Data.Repositories;
using Example.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Exemple
{
    class Program
    {
        private static readonly Random random = new Random();

        //private static string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;Pooling=true";

        static async Task Main(string[] args)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "tcp:pssc2022.database.windows.net"; 
                builder.UserID = "denis";            
                builder.Password = "Pssc2022@";     
                builder.InitialCatalog = "Students";
            try 
            {
         
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");
                    
                    connection.Open();       

                    String sql = "SELECT * FROM dbo.Product";

                    using (SqlCommand commanding = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = commanding.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader.GetString(1), reader.GetString(2));
                            }
                        }
                    }                    
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            
            using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
            ILogger<PublishGradeWorkflow> logger = loggerFactory.CreateLogger<PublishGradeWorkflow>();

            var listOfGrades = ReadListOfGrades().ToArray();
            PublishGradesCommand command = new(listOfGrades);
            var dbContextBuilder = new DbContextOptionsBuilder<GradesContext>()
                                                .UseSqlServer(builder.ConnectionString)
                                                .UseLoggerFactory(loggerFactory);
            GradesContext gradesContext = new GradesContext(dbContextBuilder.Options);
            StudentsRepository studentsRepository = new(gradesContext);
            GradesRepository gradesRepository = new(gradesContext);
            PublishGradeWorkflow workflow = new(studentsRepository, gradesRepository, logger);
            var result = await workflow.ExecuteAsync(command);

            result.Match(
                    whenExamGradesPublishFaildEvent: @event =>
                    {
                        Console.WriteLine($"Publish failed: {@event.Reason}");
                        return @event;
                    },
                    whenExamGradesPublishScucceededEvent: @event =>
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

        private static List<UnvalidatedStudentGrade> ReadListOfGrades()
        {
            List<UnvalidatedStudentGrade> listOfGrades = new();
            do
            {
                //read registration number and grade and create a list of greads
                var name = ReadValue("Nume produs: ");
                if (string.IsNullOrEmpty(name))
                {
                    break;
                }

                var quantity = ReadValue("Cantitate: ");
                if (string.IsNullOrEmpty(quantity))
                {
                    break;
                }
                string subtotal="10";

                listOfGrades.Add(new(name, quantity, subtotal));
            } while (true);
            return listOfGrades;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}
