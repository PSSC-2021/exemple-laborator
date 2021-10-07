using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lucrarea01.Domain
{
    public record PaymentAddress
    {
        
        public string Value { get; }

        public PaymentAddress(string value)
        { 
                Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
