namespace Laborator2_PSSC.Domain
{
    public record Quantity
    {
        public int Value { get; }

        public Quantity(int value)
        {
            if (is_valid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidQuantityException($"{value} is an invalid quantity value.");
            }
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public static bool TryParse(string quantityString, out Quantity quantity)
        {
            bool valid = false;
            quantity = null;
            if (int.TryParse(quantityString, out int numericQuantity))
            {
                if (is_valid(numericQuantity))
                {
                    valid = true;
                    quantity = new(numericQuantity);
                }
            }

            return valid;
        }

        private static bool is_valid(int numericQuantity) => numericQuantity > 0;
    }
}
