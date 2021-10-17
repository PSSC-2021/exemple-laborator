using Exemple.Domain.Models;
using static LanguageExt.Prelude;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Exemple.Domain.Models.ExamGrades;
using System.Threading.Tasks;

namespace Exemple.Domain
{
    public static class ExamGradesOperation
    {
        public static Task<IExamGrades> ValidateExamGrades(Func<StudentRegistrationNumber, TryAsync<bool>> checkStudentExists, UnvalidatedExamGrades examGrades) =>
            examGrades.GradeList
                      .Select(ValidateStudentGrade(checkStudentExists))
                      .Aggregate(CreateEmptyValatedGradesList().ToAsync(), ReduceValidGrades)
                      .MatchAsync(
                            Right: validatedGrades => new ValidatedExamGrades(validatedGrades),
                            LeftAsync: errorMessage => Task.FromResult((IExamGrades)new InvalidExamGrades(examGrades.GradeList, errorMessage))
                      );

        private static Func<UnvalidatedStudentGrade, EitherAsync<string, ValidatedStudentGrade>> ValidateStudentGrade(Func<StudentRegistrationNumber, TryAsync<bool>> checkStudentExists) =>
            unvalidatedStudentGrade => ValidateStudentGrade(checkStudentExists, unvalidatedStudentGrade);

        private static EitherAsync<string, ValidatedStudentGrade> ValidateStudentGrade(Func<StudentRegistrationNumber, TryAsync<bool>> checkStudentExists, UnvalidatedStudentGrade unvalidatedGrade)=>
            from examGrade in Grade.TryParseGrade(unvalidatedGrade.ExamGrade)
                                   .ToEitherAsync(() => $"Invalid exam grade ({unvalidatedGrade.StudentRegistrationNumber}, {unvalidatedGrade.ExamGrade})")
            from activityGrade in Grade.TryParseGrade(unvalidatedGrade.ActivityGrade)
                                   .ToEitherAsync(() => $"Invalid activity grade ({unvalidatedGrade.StudentRegistrationNumber}, {unvalidatedGrade.ActivityGrade})")
            from studentRegistrationNumber in StudentRegistrationNumber.TryParse(unvalidatedGrade.StudentRegistrationNumber)
                                   .ToEitherAsync(() => $"Invalid student registration number ({unvalidatedGrade.StudentRegistrationNumber})")
            from studentExists in checkStudentExists(studentRegistrationNumber)
                                   .ToEither(error => error.ToString())
            select new ValidatedStudentGrade(studentRegistrationNumber, examGrade, activityGrade);

        private static Either<string, List<ValidatedStudentGrade>> CreateEmptyValatedGradesList() =>
            Right(new List<ValidatedStudentGrade>());

        private static EitherAsync<string, List<ValidatedStudentGrade>> ReduceValidGrades(EitherAsync<string, List<ValidatedStudentGrade>> acc, EitherAsync<string, ValidatedStudentGrade> next) =>
            from list in acc
            from nextGrade in next
            select list.AppendValidGrade(nextGrade);

        private static List<ValidatedStudentGrade> AppendValidGrade(this List<ValidatedStudentGrade> list, ValidatedStudentGrade validGrade)
        {
            list.Add(validGrade);
            return list;
        }

        public static IExamGrades CalculateFinalGrades(IExamGrades examGrades) => examGrades.Match(
            whenUnvalidatedExamGrades: unvalidaTedExam => unvalidaTedExam,
            whenInvalidExamGrades: invalidExam => invalidExam,
            whenCalculatedExamGrades: calculatedExam => calculatedExam,
            whenPublishedExamGrades: publishedExam => publishedExam,
            whenValidatedExamGrades: CalculateFinalGrade
        );

        private static IExamGrades CalculateFinalGrade(ValidatedExamGrades validExamGrades) =>
            new CalculatedExamGrades(validExamGrades.GradeList
                                                    .Select(CalculateStudentFinalGrade)
                                                    .ToList()
                                                    .AsReadOnly());

        private static CalculatedSudentGrade CalculateStudentFinalGrade(ValidatedStudentGrade validGrade) => 
            new CalculatedSudentGrade(validGrade.StudentRegistrationNumber,
                                      validGrade.ExamGrade,
                                      validGrade.ActivityGrade,
                                      validGrade.ExamGrade + validGrade.ActivityGrade);

        public static IExamGrades PublishExamGrades(IExamGrades examGrades) => examGrades.Match(
            whenUnvalidatedExamGrades: unvalidaTedExam => unvalidaTedExam,
            whenInvalidExamGrades: invalidExam => invalidExam,
            whenValidatedExamGrades: validatedExam => validatedExam,
            whenPublishedExamGrades: publishedExam => publishedExam,
            whenCalculatedExamGrades: GenerateExport);

        private static IExamGrades GenerateExport(CalculatedExamGrades calculatedExam) => 
            new PublishedExamGrades(calculatedExam.GradeList, 
                                    calculatedExam.GradeList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(), 
                                    DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedSudentGrade grade) =>
            export.AppendLine($"{grade.StudentRegistrationNumber.Value}, {grade.ExamGrade}, {grade.ActivityGrade}, {grade.FinalGrade}");
    }
}
