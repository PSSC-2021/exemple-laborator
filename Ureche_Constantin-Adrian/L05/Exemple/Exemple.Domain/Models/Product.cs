using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class Product
    {
        public interface IProduct { }

        public record UnvalidateProduct: IProduct
        {
            public UnvalidateProduct(IReadOnlyCollection<UnvalidatedCart> productList)
            {
                ProductList = productList;
            }

            public IReadOnlyCollection<UnvalidatedCart> ProductList { get; }
        }

        public record InvalidProduct: IProduct
        {
            internal InvalidProduct(IReadOnlyCollection<UnvalidatedCart> productList, string reason)
            {
                ProductList = productList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedCart> ProductList { get; }
            public string Reason { get; }
        }

        public record FailedProduct : IProduct
        {
            internal FailedProduct(IReadOnlyCollection<UnvalidatedCart> productList, Exception exception)
            {
                ProductList = productList;
                Exception = exception;
            }

            public IReadOnlyCollection<UnvalidatedCart> ProductList { get; }
            public Exception Exception { get; }
        }

        public record ValidatedProduct: IProduct
        {
            internal ValidatedProduct(IReadOnlyCollection<ValidatedCart> productList)
            {
                ProductList = productList;
            }

            public IReadOnlyCollection<ValidatedCart> ProductList { get; }
        }

        public record CalculatedProduct : IProduct
        {
            internal CalculatedProduct(IReadOnlyCollection<CalculatedCart> productList)
            {
                ProductList = productList;
            }

            public IReadOnlyCollection<CalculatedCart> ProductList { get; }
        }

        public record PublishedProduct : IProduct
        {
            internal PublishedProduct(IReadOnlyCollection<CalculatedCart> productList, string csv, DateTime publishedDate)
            {
                ProductList = productList;
                PublishedDate = publishedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedCart> ProductList { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }
    }
}
