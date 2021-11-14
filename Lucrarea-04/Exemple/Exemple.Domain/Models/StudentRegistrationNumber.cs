using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record StudentRegistrationNumber
    {
        private static readonly Regex ValidPattern = new("^LM[0-9]{5}$");

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

        private static bool IsValid(string stringValue) => ValidPattern.IsMatch(stringValue);

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
