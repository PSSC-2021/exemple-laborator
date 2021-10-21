using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSSC.Domain
{
    public record Price
    {
        public decimal Value { get; }

        public Price(decimal value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidPriceException($"{value:0.##} is an invalid price value.");
            }
        }

        public static Price operator *(Price a, Quantity b) => new Price((a.Value * b.Value));

        public Price Round()
        {
            var roundedValue = Math.Round(Value);
            return new Price(roundedValue);
        }

        public override string ToString()
        {
            return $"{Value:0.##}";
        }

        public static bool TryParse(string priceString, out Price price)
        {
            bool isValid = false;
            price = null;
            if (decimal.TryParse(priceString, out decimal numericPrice))
            {
                if (IsValid(numericPrice))
                {
                    isValid = true;
                    price = new(numericPrice);
                }
            }

            return isValid;
        }

        private static bool IsValid(decimal numericPrice) => numericPrice >= 0;
    }
}
