using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain
{
    [AsChoice]
    public static partial class ExamGrades
    {
        public interface IExamGrades { }

        public record UnvalidatedExamGrades(IReadOnlyCollection<UnvalidatedStudentGrade> GradesList) : IExamGrades;

        public record InvalidatedExamGrades(IReadOnlyCollection<UnvalidatedStudentGrade> GradesList, string reason) : IExamGrades;

        public record ValidatedExamGrades(IReadOnlyCollection<ValidatedStudentGrade> GradesList) : IExamGrades;

        public record PublishedExamGrades(IReadOnlyCollection<ValidatedStudentGrade> GradesList, DateTime PublishedDate) : IExamGrades;
    }
}
