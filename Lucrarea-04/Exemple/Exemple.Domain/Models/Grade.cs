using LanguageExt;
using System;
using static LanguageExt.Prelude;

namespace Exemple.Domain.Models
{
    public record Grade
    {
        public decimal Value { get; }

        internal Grade(decimal value)
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

        public static Option<Grade> TryParseGrade(string gradeString)
        {
            if(decimal.TryParse(gradeString, out decimal numericGrade) && IsValid(numericGrade))
            {
                return Some<Grade>(new(numericGrade));
            }
            else
            {
                return None;
            }
        }

        private static bool IsValid(decimal numericGrade) => numericGrade > 0 && numericGrade <= 10;
    }
}
