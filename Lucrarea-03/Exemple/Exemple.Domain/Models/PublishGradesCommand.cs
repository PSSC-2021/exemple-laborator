using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.ExamGrades;

namespace Exemple.Domain.Models
{
    public record PublishGradesCommand
    {
        public PublishGradesCommand(IReadOnlyCollection<UnvalidatedStudentGrade> inputExamGrades)
        {
            InputExamGrades = inputExamGrades;
        }

        public IReadOnlyCollection<UnvalidatedStudentGrade> InputExamGrades { get; }
    }
}
