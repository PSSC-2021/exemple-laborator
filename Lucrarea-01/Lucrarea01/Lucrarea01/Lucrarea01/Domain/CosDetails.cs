using Lucrarea01.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lucrarea01.Domain
{
    public record CosDetails(PaymentAddress PaymentAddress, PaymentState PaymentState);
}