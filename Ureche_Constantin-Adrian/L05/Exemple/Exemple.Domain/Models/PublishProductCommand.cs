using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.Product;

namespace Exemple.Domain.Models
{
    public record PublishProductCommand
    {
        public PublishProductCommand(IReadOnlyCollection<UnvalidatedCart> inputProduct)
        {
            InputProduct = inputProduct;
        }

        public IReadOnlyCollection<UnvalidatedCart> InputProduct { get; }
    }
}
