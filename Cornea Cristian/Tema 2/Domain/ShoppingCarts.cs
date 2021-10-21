using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Laborator2_PSSC.Domain
{
    [AsChoice]
    public static partial class ShoppingCarts
    {
        public interface IShoppingCarts { }

        public record EmptyShoppingCarts : IShoppingCarts
        {
            public EmptyShoppingCarts(IReadOnlyCollection<EmptyShoppingCart> shoppingCartList)
            {
                ShoppingCartList = shoppingCartList;
            }

            public IReadOnlyCollection<EmptyShoppingCart> ShoppingCartList { get; }
        }

        public record UnvalidatedShoppingCarts : IShoppingCarts
        {
            internal UnvalidatedShoppingCarts(IReadOnlyCollection<EmptyShoppingCart> shoppingCartList, string reason)
            {
                ShoppingCartList = shoppingCartList;
                Reason = reason;
            }

            public IReadOnlyCollection<EmptyShoppingCart> ShoppingCartList { get; }
            public string Reason { get; }
        }

        public record ValidatedShoppingCarts : IShoppingCarts
        {
            internal ValidatedShoppingCarts(IReadOnlyCollection<ValidatedShoppingCart> shoppingCartList)
            {
                ShoppingCartList = shoppingCartList;
            }

            public IReadOnlyCollection<ValidatedShoppingCart> ShoppingCartList { get; }
        }

        public record CalculatedShoppingCarts : IShoppingCarts
        {
            internal CalculatedShoppingCarts(IReadOnlyCollection<CalculatedShoppingCart> shoppingCartList)
            {
                ShoppingCartList = shoppingCartList;
            }

            public IReadOnlyCollection<CalculatedShoppingCart> ShoppingCartList { get; }
        }

        public record PaidShoppingCarts : IShoppingCarts
        {
            internal PaidShoppingCarts(IReadOnlyCollection<CalculatedShoppingCart> shoppingCartsList, string csv, DateTime publishedDate)
            {
                ShoppingCartList = shoppingCartsList;
                PublishedDate = publishedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedShoppingCart> ShoppingCartList { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }
    }
}
