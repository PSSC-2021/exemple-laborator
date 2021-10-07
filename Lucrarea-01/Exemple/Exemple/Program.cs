using Exemple.Domain;
using System;
using System.Collections.Generic;
using static Exemple.Domain.ExamGrades;

namespace Exemple
{
    class Program
    {
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            var listOfGrades = ReadListOfGrades().ToArray();
            UnvalidatedExamGrades unvalidatedGrades = new(listOfGrades);
            IExamGrades result = ValidateExamGrades(unvalidatedGrades);

            result.Match(
                whenUnvalidatedExamGrades: unvalidatedResult => unvalidatedGrades,
                whenPublishedExamGrades: publishedResult => publishedResult,
                whenInvalidatedExamGrades: invalidResult => invalidResult,
                whenValidatedExamGrades: validatedResult => PublishExamGrades(validatedResult)
            );
 
            foreach(var item in listOfGrades)
            {
                Console.WriteLine(item.StudentRegistrationNumber);
                Console.WriteLine(item.Grade);
            }
        }

        private static List<UnvalidatedStudentGrade> ReadListOfGrades()
        {
            List <UnvalidatedStudentGrade> listOfGrades = new();
            do
            {
                //read registration number and grade and create a list of greads
                var registrationNumber = ReadValue("Registration Number: ");
                if (string.IsNullOrEmpty(registrationNumber))
                {
                    break;
                }

                var grade = ReadValue("Grade: ");
                if (string.IsNullOrEmpty(grade))
                {
                    break;
                }

                listOfGrades.Add(new (registrationNumber, grade));
            } while (true);
            return listOfGrades;
        }

        private static IExamGrades ValidateExamGrades(UnvalidatedExamGrades unvalidatedGrades) =>
            random.Next(100) > 50 ?
            new InvalidatedExamGrades(new List<UnvalidatedStudentGrade>(), "Random errror")
            : new ValidatedExamGrades(new List<ValidatedStudentGrade>());

        private static IExamGrades PublishExamGrades(ValidatedExamGrades validExamGrades)
        {
            Console.WriteLine("Publicat");          
            return new PublishedExamGrades(new List<ValidatedStudentGrade>(), DateTime.Now);
        }
        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
