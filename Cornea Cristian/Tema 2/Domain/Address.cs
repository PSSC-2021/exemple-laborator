using System.Text.RegularExpressions;

namespace Laborator2_PSSC.Domain
{
    public record Address
    {
        private static readonly Regex ValidPattern = new("^.*$");

        public string _address { get; }

        public Address(string address)
        {
            if (ValidPattern.IsMatch(address))
            {
                _address = address;
            }
            else
            {
                throw new InvalidAddressException("");
            }
        }

        public override string ToString()
        {
            return _address;
        }

        private static bool is_valid(string stringValue) => ValidPattern.IsMatch(stringValue);

        public static bool TryParse(string addressString, out Address address)
        {
            bool valid = false;
            address = null;
            if (is_valid(addressString))
            {
                valid = true;
                address = new(addressString);
            }
            return valid;
        }

    }
}
