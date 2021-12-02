using LanguageExt;
using System;
using static LanguageExt.Prelude;

namespace Exemple.Domain.Models
{
    public record Cantity
    {
        public decimal Value { get; }

        internal Cantity(decimal value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidProductException($"{value:0.##} is an invalid Cantity value.");
            }
        }

        //public static Price operator +(Price a, Cantity b) => new Price((a.Value + b.Value) / 2m);


        public Cantity Round()
        {
            var roundedValue = Math.Round(Value);
            return new Cantity(roundedValue);
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }

        public static Option<Cantity> TryParseCantity(string cantityString)
        {
            if(decimal.TryParse(cantityString, out decimal numericCantity) && IsValid(numericCantity))
            {
                return Some<Cantity>(new(numericCantity));
            }
            else
            {
                return None;
            }
        }

        private static bool IsValid(decimal numericCantity) => numericCantity > 0 && numericCantity <= 10;
    }
}
