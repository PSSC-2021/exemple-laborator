using System.Text.RegularExpressions;

namespace Laborator1.Domain
{
    public record ProductCode
    {
        private static readonly Regex ValidPattern = new("^1[0-9]{3}$");

        public string Code { get; }

        private ProductCode(string value)
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
    }
}
