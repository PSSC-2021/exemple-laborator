using Exemple.Domain.Models;
using static Exemple.Domain.Models.ExamGradesPublishedEvent;
using static Exemple.Domain.ExamGradesOperation;
using System;
using static Exemple.Domain.Models.ExamGrades;
using LanguageExt;
using System.Threading.Tasks;
using System.Collections.Generic;
using Exemple.Domain.Repositories;
using System.Linq;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;
using Example.Events;
using Example.Dto.Events;
using Example.Dto.Models;

namespace Exemple.Domain
{
    public class PublishGradeWorkflow
    {
        private readonly IStudentsRepository studentsRepository;
        private readonly IGradesRepository gradesRepository;
        private readonly ILogger<PublishGradeWorkflow> logger;
        private readonly IEventSender eventSender;

        public PublishGradeWorkflow(IStudentsRepository studentsRepository, IGradesRepository gradesRepository,
                                    ILogger<PublishGradeWorkflow> logger, IEventSender eventSender)
        {
            this.studentsRepository = studentsRepository;
            this.gradesRepository = gradesRepository;
            this.logger = logger;
            this.eventSender = eventSender;
        }

        public async Task<IExamGradesPublishedEvent> ExecuteAsync(PublishGradesCommand command)
        {
            UnvalidatedExamGrades unvalidatedGrades = new UnvalidatedExamGrades(command.InputExamGrades);

            var result = from students in studentsRepository.TryGetExistingStudents(unvalidatedGrades.GradeList.Select(grade => grade.StudentRegistrationNumber))
                                          .ToEither(ex => new FailedExamGrades(unvalidatedGrades.GradeList, ex) as IExamGrades)
                         from existingGrades in gradesRepository.TryGetExistingGrades()
                                          .ToEither(ex => new FailedExamGrades(unvalidatedGrades.GradeList, ex) as IExamGrades)
                         let checkStudentExists = (Func<StudentRegistrationNumber, Option<StudentRegistrationNumber>>)(student => CheckStudentExists(students, student))
                         from publishedGrades in ExecuteWorkflowAsync(unvalidatedGrades, existingGrades, checkStudentExists).ToAsync()
                         from saveResult in gradesRepository.TrySaveGrades(publishedGrades)
                                          .ToEither(ex => new FailedExamGrades(unvalidatedGrades.GradeList, ex) as IExamGrades)
                         let grades = publishedGrades.GradeList.Select(grade => new PublishedStudentGrade(
                                                        grade.StudentRegistrationNumber,
                                                        ExamGrade: grade.ExamGrade,
                                                        ActivityGrade: grade.ActivityGrade,
                                                        FinalGrade: grade.FinalGrade))
                         let successfulEvent = new ExamGradesPublishScucceededEvent(grades, publishedGrades.PublishedDate)
                         let eventToPublish = new GradesPublishedEvent()
                         {
                             Grades = grades.Select(g=>new StudentGradeDto()
                             {
                                 Name = g.StudentRegistrationNumber.Value, //TODO get name here
                                 StudentRegistrationNumber = g.StudentRegistrationNumber.Value,
                                 ActivityGrade = g.ActivityGrade.Value,
                                 ExamGrade = g.ExamGrade.Value,
                                 FinalGrade = g.FinalGrade.Value
                                 
                             }).ToList()
                         }
                         from publishEventResult in eventSender.SendAsync("grades", eventToPublish)
                                              .ToEither(ex => new FailedExamGrades(unvalidatedGrades.GradeList, ex) as IExamGrades)
                         select successfulEvent;

            return await result.Match(
                    Left: examGrades => GenerateFailedEvent(examGrades) as IExamGradesPublishedEvent,
                    Right: publishedGrades => publishedGrades
                );
        }

        private async Task<Either<IExamGrades, PublishedExamGrades>> ExecuteWorkflowAsync(UnvalidatedExamGrades unvalidatedGrades,
                                                                                          IEnumerable<CalculatedSudentGrade> existingGrades,
                                                                                          Func<StudentRegistrationNumber, Option<StudentRegistrationNumber>> checkStudentExists)
        {

            IExamGrades grades = await ValidateExamGrades(checkStudentExists, unvalidatedGrades);
            grades = CalculateFinalGrades(grades);
            grades = MergeGrades(grades, existingGrades);
            grades = PublishExamGrades(grades);

            return grades.Match<Either<IExamGrades, PublishedExamGrades>>(
                whenUnvalidatedExamGrades: unvalidatedGrades => Left(unvalidatedGrades as IExamGrades),
                whenCalculatedExamGrades: calculatedGrades => Left(calculatedGrades as IExamGrades),
                whenInvalidExamGrades: invalidGrades => Left(invalidGrades as IExamGrades),
                whenFailedExamGrades: failedGrades => Left(failedGrades as IExamGrades),
                whenValidatedExamGrades: validatedGrades => Left(validatedGrades as IExamGrades),
                whenPublishedExamGrades: publishedGrades => Right(publishedGrades)
            );
        }

        private Option<StudentRegistrationNumber> CheckStudentExists(IEnumerable<StudentRegistrationNumber> students, StudentRegistrationNumber studentRegistrationNumber)
        {
            if (students.Any(s => s == studentRegistrationNumber))
            {
                return Some(studentRegistrationNumber);
            }
            else
            {
                return None;
            }
        }

        private ExamGradesPublishFaildEvent GenerateFailedEvent(IExamGrades examGrades) =>
            examGrades.Match<ExamGradesPublishFaildEvent>(
                whenUnvalidatedExamGrades: unvalidatedExamGrades => new($"Invalid state {nameof(UnvalidatedExamGrades)}"),
                whenInvalidExamGrades: invalidExamGrades => new(invalidExamGrades.Reason),
                whenValidatedExamGrades: validatedExamGrades => new($"Invalid state {nameof(ValidatedExamGrades)}"),
                whenFailedExamGrades: failedExamGrades =>
                {
                    logger.LogError(failedExamGrades.Exception, failedExamGrades.Exception.Message);
                    return new(failedExamGrades.Exception.Message);
                },
                whenCalculatedExamGrades: calculatedExamGrades => new($"Invalid state {nameof(CalculatedExamGrades)}"),
                whenPublishedExamGrades: publishedExamGrades => new($"Invalid state {nameof(PublishedExamGrades)}"));
    }
}
