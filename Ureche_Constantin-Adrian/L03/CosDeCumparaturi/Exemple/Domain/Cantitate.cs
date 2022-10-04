using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt;

namespace CosDeCumparaturi.Domain
{
    public record Cantitate
    {
        public decimal Value { get; }

        public Cantitate(decimal value)
        {
            Value = value;

        }

        
        public static Option<Cantitate> TryParse(string cantitateStr)
        {
            if(decimal.TryParse(cantitateStr, out decimal numericCant) && IsValid(numericCant))
            {
                return Some<Cantitate>(new(numericCant));
            }
            else
            {
                return None;
            }
        }
        private static bool IsValid(decimal numericGrade) => numericGrade > 0 && numericGrade <= 10;


        public override string ToString()
        {
            return $"{Value:0.##}";
        }
    }
}
