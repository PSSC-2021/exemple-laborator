using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosDeCumparaturi.Domain
{

    public record Adresa
    {
        public string Value { get; }

        public Adresa(string value)
        {
            Value = value;

        }


        public override string ToString()
        {
            return Value;
        }

    }
}
