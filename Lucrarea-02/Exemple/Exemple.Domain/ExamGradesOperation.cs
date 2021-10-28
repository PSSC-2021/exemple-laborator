using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.ExamGrades;

namespace Exemple.Domain
{
    public static class ExamGradesOperation
    {
        public static IExamGrades ValidateExamGrades(Func<StudentRegistrationNumber, bool> checkStudentExists, UnvalidatedExamGrades examGrades)
        {
            List<ValidatedStudentGrade> validatedGrades = new();
            bool isValidList = true;
            string invalidReson = string.Empty;
            foreach(var unvalidatedGrade in examGrades.GradeList)
            {
                if (!Grade.TryParseGrade(unvalidatedGrade.ExamGrade, out Grade examGrade))
                {
                    invalidReson = $"Invalid exam grade ({unvalidatedGrade.StudentRegistrationNumber}, {unvalidatedGrade.ExamGrade})";
                    isValidList = false;
                    break;
                }
                if (!Grade.TryParseGrade(unvalidatedGrade.ActivityGrade, out Grade activityGrade))
                {
                    invalidReson = $"Invalid activity grade ({unvalidatedGrade.StudentRegistrationNumber}, {unvalidatedGrade.ActivityGrade})";
                    isValidList = false;
                    break;
                }
                if (!StudentRegistrationNumber.TryParse(unvalidatedGrade.StudentRegistrationNumber, out StudentRegistrationNumber studentRegistrationNumber)
                    && checkStudentExists(studentRegistrationNumber))
                {
                    invalidReson = $"Invalid student registration number ({unvalidatedGrade.StudentRegistrationNumber})";
                    isValidList = false;
                    break;
                }
                ValidatedStudentGrade validGrade = new(studentRegistrationNumber, examGrade, activityGrade);
                validatedGrades.Add(validGrade);
            }

            if (isValidList)
            {
                return new ValidatedExamGrades(validatedGrades);
            }
            else
            {
                return new InvalidatedExamGrades(examGrades.GradeList, invalidReson);
            }

        }

        public static IExamGrades CalculateFinalGrades(IExamGrades examGrades) => examGrades.Match(
            whenUnvalidatedExamGrades: unvalidaTedExam => unvalidaTedExam,
            whenInvalidatedExamGrades: invalidExam => invalidExam,
            whenCalculatedExamGrades: calculatedExam => calculatedExam,
            whenPublishedExamGrades: publishedExam => publishedExam,
            whenValidatedExamGrades: validExamGrades =>
            {
                var calculatedGrade = validExamGrades.GradeList.Select(validGrade =>
                                            new CalculatedSudentGrade(validGrade.StudentRegistrationNumber,
                                                                      validGrade.ExamGrade,
                                                                      validGrade.ActivityGrade,
                                                                      validGrade.ExamGrade + validGrade.ActivityGrade));
                return new CalculatedExamGrades(calculatedGrade.ToList().AsReadOnly());
            }
        );

        public static IExamGrades PublishExamGrades(IExamGrades examGrades) => examGrades.Match(
            whenUnvalidatedExamGrades: unvalidaTedExam => unvalidaTedExam,
            whenInvalidatedExamGrades: invalidExam => invalidExam,
            whenValidatedExamGrades: validatedExam => validatedExam,
            whenPublishedExamGrades: publishedExam => publishedExam,
            whenCalculatedExamGrades: calculatedExam =>
            {
                StringBuilder csv = new();
                calculatedExam.GradeList.Aggregate(csv, (export, grade) => export.AppendLine($"{grade.StudentRegistrationNumber.Value}, {grade.ExamGrade}, {grade.ActivityGrade}, , {grade.FinalGrade}"));

                PublishedExamGrades publishedExamGrades = new(calculatedExam.GradeList, csv.ToString(), DateTime.Now);

                return publishedExamGrades;
            });
    }
}
