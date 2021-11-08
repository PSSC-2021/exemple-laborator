using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosDeCumparaturi.Domain
{
    public record StareCalculat(CodProdus cod, Cantitate cantitate, Adresa adresa, Pret pret);
}
