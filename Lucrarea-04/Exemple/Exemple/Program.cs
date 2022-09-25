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

namespace Exemple
{
    class Program
    {
        private static readonly Random random = new Random();

        static async Task Main(string[] args)
        {
            //var testNumber = StudentRegistrationNumber.TryParse("LM12345");
            //var studentExists = await testNumber.Match(
            //    Some: testNumber => CheckStudentExists(testNumber).Match(Succ: value => value, exception=>false),
            //    None: () => Task.FromResult(false)
            //);

            //var result = from studentNumber in StudentRegistrationNumber.TryParse("LM12345")
            //                                        .ToEitherAsync(() => "Invlid student registration number.")
            //             from exists in CheckStudentExists(studentNumber)
            //                                        .ToEither(ex =>
            //                                        {
            //                                            Console.Error.WriteLine(ex.ToString());
            //                                            return "Could not validate student reg. number";
            //                                        })
            //             select exists;

            //await result.Match(
            //     Left: message => Console.WriteLine(message),
            //     Right: flag => Console.WriteLine(flag));



            var listOfGrades = ReadListOfGrades().ToArray();
            PublishGradesCommand command = new(listOfGrades);
            PublishGradeWorkflow workflow = new();
            var result = await workflow.ExecuteAsync(command, CheckStudentExists);

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

        private static int Sum()
        {
            Option<int> two = Some(2);
            Option<int> four = Some(4);
            Option<int> six = Some(6);
            Option<int> none = None;

            var result = from x in two
                         from y in four
                         from z in six
                         from n in none
                         select x + y + z + n;
            // This expression succeeds because all items to the right of 'in' are Some of int
            // and therefore it lands in the Some lambda.
            int r = match(result,
                           Some: v => v * 2,
                           None: () => 0);     // r == 24

            return r;
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

        private static TryAsync<bool> CheckStudentExists(StudentRegistrationNumber student) {
            Func<Task<bool>> func = async () =>
                 {
                     //HttpClient client = new HttpClient();

                     //var response = await client.PostAsync($"www.university.com/checkRegistrationNumber?number={student.Value}", new StringContent(""));

                     //response.EnsureSuccessStatusCode(); //200

                     return true;
                 };
            return TryAsync(func);
        }
    }
}
