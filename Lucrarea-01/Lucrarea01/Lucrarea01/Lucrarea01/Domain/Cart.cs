using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucrarea01.Domain
{
    [AsChoice]
    public static partial class Cart
    {
        public interface ICart
        {   }

        public record UnvalidatedCart(IReadOnlyCollection<UnvalidatedProducts> ProductsList, CartDetails CartDetails) : ICart;

        public record InvalidatedCart(IReadOnlyCollection<UnvalidatedProducts> ProductsList, string reason) : ICart;

        public record EmptyCart() : ICart;
        
        public record ValidatedCart(IReadOnlyCollection<ValidatedProducts> ProductsList, CartDetails CartDetails) : ICart;

        public record PaidCart(IReadOnlyCollection<ValidatedProducts> ProductsList, CartDetails CartDetails, DateTime PublishedDate) : ICart;

    }
}
