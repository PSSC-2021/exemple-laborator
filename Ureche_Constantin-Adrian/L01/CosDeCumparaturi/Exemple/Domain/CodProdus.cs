using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosDeCumparaturi.Domain
{
    public record CodProdus
    {
        public int Value { get; }

        public CodProdus(int value)
        {
            Value = value;

        }


        public override string ToString()
        {
            return $"{Value:0.##}";
        }
    }
}
