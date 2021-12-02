using LanguageExt;
using static LanguageExt.Prelude;
using System.Text.RegularExpressions;

namespace Exemple.Domain.Models
{
    public record StudentRegistrationNumber
    {
        public const string Pattern = "^LM[0-9]{5}$";
        private static readonly Regex PatternRegex = new(Pattern);

        public string Value { get; }

        internal StudentRegistrationNumber(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidStudentRegistrationNumberException("");
            }
        }

        private static bool IsValid(string stringValue) => PatternRegex.IsMatch(stringValue);

        public override string ToString()
        {
            return Value;
        }

        public static Option<StudentRegistrationNumber> TryParse(string stringValue)
        {
            if (IsValid(stringValue))
            {
                return Some<StudentRegistrationNumber>(new(stringValue));
            }
            else
            {
                return None;
            }
        }
    }
}
