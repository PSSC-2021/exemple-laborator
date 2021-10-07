using System.Text.RegularExpressions;

namespace Laborator1.Domain
{
    class Address
    {
        private static readonly Regex ValidPattern = new("{str, nr}");

        public string _address { get; }

        private Address(string address)
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
    }
}
