using Exemple.Domain.Models;
using static Exemple.Domain.Models.ExamGradesPublishedEvent;
using static Exemple.Domain.ExamGradesOperation;
using System;
using static Exemple.Domain.Models.ExamGrades;

namespace Exemple.Domain
{
    public class PublishGradeWorkflow
    {
        public IExamGradesPublishedEvent Execute(PublishGradesCommand command, Func<StudentRegistrationNumber, bool> checkStudentExists)
        {
            UnvalidatedExamGrades unvalidatedGrades = new UnvalidatedExamGrades(command.InputExamGrades);
            IExamGrades grades = ValidateExamGrades(checkStudentExists, unvalidatedGrades);
            grades = CalculateFinalGrades(grades);
            grades = PublishExamGrades(grades);

            return grades.Match(
                    whenUnvalidatedExamGrades: unvalidatedGrades => new ExamGradesPublishFaildEvent("Unexpected unvalidated state") as IExamGradesPublishedEvent,
                    whenInvalidatedExamGrades: invalidGrades => new ExamGradesPublishFaildEvent(invalidGrades.Reason),
                    whenValidatedExamGrades: validatedGrades => new ExamGradesPublishFaildEvent("Unexpected validated state"),
                    whenCalculatedExamGrades: calculatedGrades => new ExamGradesPublishFaildEvent("Unexpected calculated state"),
                    whenPublishedExamGrades: publishedGrades => new ExamGradesPublishScucceededEvent(publishedGrades.Csv, publishedGrades.PublishedDate)
                );
        }
    }
}
