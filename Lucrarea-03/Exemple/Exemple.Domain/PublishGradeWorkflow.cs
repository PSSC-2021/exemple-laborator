using Exemple.Domain.Models;
using static Exemple.Domain.Models.ExamGradesPublishedEvent;
using static Exemple.Domain.ExamGradesOperation;
using System;
using static Exemple.Domain.Models.ExamGrades;
using LanguageExt;
using System.Threading.Tasks;

namespace Exemple.Domain
{
    public class PublishGradeWorkflow
    {
        public async Task<IExamGradesPublishedEvent> ExecuteAsync(PublishGradesCommand command, Func<StudentRegistrationNumber, TryAsync<bool>> checkStudentExists)
        {
            UnvalidatedExamGrades unvalidatedGrades = new UnvalidatedExamGrades(command.InputExamGrades);
            IExamGrades grades = await ValidateExamGrades(checkStudentExists, unvalidatedGrades);
            grades = CalculateFinalGrades(grades);
            grades = PublishExamGrades(grades);

            return grades.Match(
                    whenUnvalidatedExamGrades: unvalidatedGrades => new ExamGradesPublishFaildEvent("Unexpected unvalidated state") as IExamGradesPublishedEvent,
                    whenInvalidExamGrades: invalidGrades => new ExamGradesPublishFaildEvent(invalidGrades.Reason),
                    whenValidatedExamGrades: validatedGrades => new ExamGradesPublishFaildEvent("Unexpected validated state"),
                    whenCalculatedExamGrades: calculatedGrades => new ExamGradesPublishFaildEvent("Unexpected calculated state"),
                    whenPublishedExamGrades: publishedGrades => new ExamGradesPublishScucceededEvent(publishedGrades.Csv, publishedGrades.PublishedDate)
                );
        }
    }
}
