using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt;

namespace CosDeCumparaturi.Domain
{

    public record Adresa
    {
        public string Value { get; }

        public Adresa(string value)
        {
            Value = value;

        }

        public static Option<Adresa> TryParse(string pretStr)
        {
            return Some<Adresa>(new(pretStr));
        }


        public override string ToString()
        {
            return Value;
        }

    }
}
