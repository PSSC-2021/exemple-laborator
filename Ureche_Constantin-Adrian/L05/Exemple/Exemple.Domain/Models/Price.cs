using LanguageExt;
using System;
using static LanguageExt.Prelude;

namespace Exemple.Domain.Models
{
    public record Price
    {
        public decimal Value { get; }

        internal Price(decimal value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidProductException($"{value:0.##} is an invalid Price value.");
            }
        }

        public static Price operator *(Price a, Cantity b) => new Price((a.Value * b.Value) / 2m);


        public Price Round()
        {
            var roundedValue = Math.Round(Value);
            return new Price(roundedValue);
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }

        public static Option<Price> TryParsePrice(string priceString)
        {
            if(decimal.TryParse(priceString, out decimal numericPrice) && IsValid(numericPrice))
            {
                return Some<Price>(new(numericPrice));
            }
            else
            {
                return None;
            }
        }

        private static bool IsValid(decimal numericPrice) => numericPrice > 0 && numericPrice <= 10;
    }
}
