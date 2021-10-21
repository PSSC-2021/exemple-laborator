using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain
{
    public record Grade
    {
        public decimal Value { get; }

        public Grade(decimal value)
        {
            if (value > 0 && value <= 10)
            {
                Value = value;
            }
            else
            {
                throw new InvalidGradeException($"{value:0.##} is an invalid grade value.");
            }
        }

        public Grade Round()
        {
            var roundedValue = Math.Round(Value);
            return new Grade(roundedValue);
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }
    }
}
