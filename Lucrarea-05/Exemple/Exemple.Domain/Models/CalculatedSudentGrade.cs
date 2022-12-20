using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record CalculatedSudentGrade(StudentRegistrationNumber Name, Grade Quantity, Grade Subtotal, Grade Total)
    {
        public int CommandId { get; set; }
        public bool IsUpdated { get; set; } 
    }
}
