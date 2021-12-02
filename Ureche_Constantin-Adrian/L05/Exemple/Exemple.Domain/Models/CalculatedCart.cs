using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record CalculatedCart(CartCod CartCod, Price Price, Cantity Cantity, Price TotalPrice)
    {
        public int ProductCod { get; set; }
        public bool IsUpdated { get; set; } 
    }
}
