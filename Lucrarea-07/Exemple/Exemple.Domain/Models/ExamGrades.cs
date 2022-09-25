using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class ExamGrades
    {
        public interface IExamGrades { }

        public record UnvalidatedExamGrades: IExamGrades
        {
            public UnvalidatedExamGrades(IReadOnlyCollection<UnvalidatedStudentGrade> gradeList)
            {
                GradeList = gradeList;
            }

            public IReadOnlyCollection<UnvalidatedStudentGrade> GradeList { get; }
        }

        public record InvalidExamGrades: IExamGrades
        {
            internal InvalidExamGrades(IReadOnlyCollection<UnvalidatedStudentGrade> gradeList, string reason)
            {
                GradeList = gradeList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedStudentGrade> GradeList { get; }
            public string Reason { get; }
        }

        public record FailedExamGrades : IExamGrades
        {
            internal FailedExamGrades(IReadOnlyCollection<UnvalidatedStudentGrade> gradeList, Exception exception)
            {
                GradeList = gradeList;
                Exception = exception;
            }

            public IReadOnlyCollection<UnvalidatedStudentGrade> GradeList { get; }
            public Exception Exception { get; }
        }

        public record ValidatedExamGrades: IExamGrades
        {
            internal ValidatedExamGrades(IReadOnlyCollection<ValidatedStudentGrade> gradesList)
            {
                GradeList = gradesList;
            }

            public IReadOnlyCollection<ValidatedStudentGrade> GradeList { get; }
        }

        public record CalculatedExamGrades : IExamGrades
        {
            internal CalculatedExamGrades(IReadOnlyCollection<CalculatedSudentGrade> gradesList)
            {
                GradeList = gradesList;
            }

            public IReadOnlyCollection<CalculatedSudentGrade> GradeList { get; }
        }

        public record PublishedExamGrades : IExamGrades
        {
            internal PublishedExamGrades(IReadOnlyCollection<CalculatedSudentGrade> gradesList, string csv, DateTime publishedDate)
            {
                GradeList = gradesList;
                PublishedDate = publishedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedSudentGrade> GradeList { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }
    }
}
