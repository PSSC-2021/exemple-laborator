using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Exemple.Domain.Models;

namespace Example.Api.Models
{
    public class InputGrade
    {
        [Required]
        [RegularExpression(StudentRegistrationNumber.Pattern)]
        public string RegistrationNumber { get; set; }

        [Required]
        [Range(1, 10)]
        public decimal Exam { get; set; }

        [Required]
        [Range(1, 10)]
        public decimal Activity { get; set; }
    }
}
