using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Laborator1.Domain
{
    [AsChoice]
    public static partial class ShoppingCarts
    {
        public interface IShoppingCarts { }

        public record EmptyShoppingCarts(IReadOnlyCollection<EmptyShoppingCart> ShoppingCarts) : IShoppingCarts;

        public record UnvalidatedShoppingCarts(IReadOnlyCollection<UnvalidatedShoppingCart> ShoppingCarts, string reason) : IShoppingCarts;

        public record ValidatedShoppingCarts(IReadOnlyCollection<ValidatedShoppingCart> ShoppingCarts) : IShoppingCarts;

        public record PaidShoppingCarts(IReadOnlyCollection<ValidatedShoppingCart> ShoppingCarts, DateTime PublishedDate) : IShoppingCarts;
    }
}
