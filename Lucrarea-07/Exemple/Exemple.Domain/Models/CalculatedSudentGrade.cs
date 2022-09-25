using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record CalculatedSudentGrade(StudentRegistrationNumber StudentRegistrationNumber, Grade ExamGrade, Grade ActivityGrade, Grade FinalGrade)
    {
        public int GradeId { get; set; }
        public bool IsUpdated { get; set; } 
    }
}
