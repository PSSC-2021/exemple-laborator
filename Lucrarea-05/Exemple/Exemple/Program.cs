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

namespace Exemple
{
    class Program
    {
        private static readonly Random random = new Random();

        private static string ConnectionString = "Server=LAPTOP-5O6G7HEC\\DEVELOPER;Database=PSSC-sample;Trusted_Connection=True;MultipleActiveResultSets=true";

        static async Task Main(string[] args)
        {
            using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
            ILogger<PublishGradeWorkflow> logger = loggerFactory.CreateLogger<PublishGradeWorkflow>();

            var listOfGrades = ReadListOfGrades().ToArray();
            PublishGradesCommand command = new(listOfGrades);
            var dbContextBuilder = new DbContextOptionsBuilder<GradesContext>()
                                                .UseSqlServer(ConnectionString)
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
                var registrationNumber = ReadValue("Registration Number: ");
                if (string.IsNullOrEmpty(registrationNumber))
                {
                    break;
                }

                var examGrade = ReadValue("Exam Grade: ");
                if (string.IsNullOrEmpty(examGrade))
                {
                    break;
                }

                var activityGrade = ReadValue("Activity Grade: ");
                if (string.IsNullOrEmpty(activityGrade))
                {
                    break;
                }

                listOfGrades.Add(new(registrationNumber, examGrade, activityGrade));
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
