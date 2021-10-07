using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record Grade
    {
        public decimal Value { get; }

        public Grade(decimal value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidGradeException($"{value:0.##} is an invalid grade value.");
            }
        }

        public static Grade operator +(Grade a, Grade b) => new Grade((a.Value + b.Value) / 2m);


        public Grade Round()
        {
            var roundedValue = Math.Round(Value);
            return new Grade(roundedValue);
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }

        public static bool TryParseGrade(string gradeString, out Grade grade)
        {
            bool isValid = false;
            grade = null;
            if(decimal.TryParse(gradeString, out decimal numericGrade))
            {
                if (IsValid(numericGrade))
                {
                    isValid = true;
                    grade = new(numericGrade);
                }
            }

            return isValid;
        }

        private static bool IsValid(decimal numericGrade) => numericGrade > 0 && numericGrade <= 10;
    }
}
