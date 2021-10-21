using System.Text.RegularExpressions;

namespace Laborator2_PSSC.Domain
{
    public record ProductCode
    {
        private static readonly Regex ValidPattern = new("^.*$");

        public string Code { get; }

        public ProductCode(string value)
        {
            if (ValidPattern.IsMatch(value))
            {
                Code = value;
            }
            else
            {
                throw new InvalidProductCodeException("");
            }
        }

        public override string ToString()
        {
            return Code;
        }

        private static bool is_valid(string stringValue) => ValidPattern.IsMatch(stringValue);

        public static bool TryParse(string productCodeString, out ProductCode productCode)
        {
            bool isValid = false;
            productCode = null;
            if (is_valid(productCodeString))
            {
                isValid = true;
                productCode = new(productCodeString);
            }
            return isValid;
        }
    }
}
