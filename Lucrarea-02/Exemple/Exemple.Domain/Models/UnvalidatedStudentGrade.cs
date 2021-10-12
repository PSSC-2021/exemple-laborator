using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record UnvalidatedStudentGrade(string StudentRegistrationNumber, string ExamGrade, string ActivityGrade);
}
