using System;

namespace Laborator2_PSSC.Domain
{
    public record Price
    {
        public decimal Value { get; }

        public Price(decimal value)
        {
            if (is_valid(value))
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
            return new Price(Math.Round(Value));
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
                if (is_valid(numericPrice))
                {
                    isValid = true;
                    price = new(numericPrice);
                }
            }

            return isValid;
        }

        private static bool is_valid(decimal numericPrice) => numericPrice >= 0;
    }
}
