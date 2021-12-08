using Exemple.Domain.Models;
using static LanguageExt.Prelude;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Exemple.Domain.Models.Product;
using System.Threading.Tasks;

namespace Exemple.Domain
{
    public static class ProductOperation
    {
        public static Task<IProduct> ValidateProduct(Func<CartCod, Option<CartCod>> checkCartExists, UnvalidateProduct product) =>
            product.ProductList
                      .Select(ValidateCart(checkCartExists))
                      .Aggregate(CreateEmptyValatedProductList().ToAsync(), ReduceValidProduct)
                      .MatchAsync(
                            Right: validatedProduct => new ValidatedProduct(validatedProduct),
                            LeftAsync: errorMessage => Task.FromResult((IProduct)new InvalidProduct(product.ProductList, errorMessage))
                      );

        private static Func<UnvalidatedCart, EitherAsync<string, ValidatedCart>> ValidateCart(Func<CartCod, Option<CartCod>> checkCart) =>
            unvalidatedCart => ValidateCart(checkCart, unvalidatedCart);

        private static EitherAsync<string, ValidatedCart> ValidateCart(Func<CartCod, Option<CartCod>> checkStudentExists, UnvalidatedCart unvalidatedGrade)=>
            from price in Price.TryParsePrice(unvalidatedGrade.Price)
                                   .ToEitherAsync($"Invalid price ({unvalidatedGrade.CartCod}, {unvalidatedGrade.Price})")
            from cantity in Cantity.TryParseCantity(unvalidatedGrade.Cantity)
                                   .ToEitherAsync($"Invalid cantity ({unvalidatedGrade.CartCod}, {unvalidatedGrade.Cantity})")
            from cartCod in CartCod.TryParse(unvalidatedGrade.CartCod)
                                   .ToEitherAsync($"Invalid cod ({unvalidatedGrade.CartCod})")
            from cartExists in checkStudentExists(cartCod)
                                   .ToEitherAsync($"Cart {cartCod.Value} does not exist.")
            select new ValidatedCart(cartCod, price, cantity);

        private static Either<string, List<ValidatedCart>> CreateEmptyValatedProductList() =>
            Right(new List<ValidatedCart>());

        private static EitherAsync<string, List<ValidatedCart>> ReduceValidProduct(EitherAsync<string, List<ValidatedCart>> acc, EitherAsync<string, ValidatedCart> next) =>
            from list in acc
            from nextGrade in next
            select list.AppendValidProduct(nextGrade);

        private static List<ValidatedCart> AppendValidProduct(this List<ValidatedCart> list, ValidatedCart validGrade)
        {
            list.Add(validGrade);
            return list;
        }

        public static IProduct CalculateFinalProduct(IProduct product) => product.Match(
            whenUnvalidateProduct: unvalidaTedExam => unvalidaTedExam,
            whenInvalidProduct: invalidExam => invalidExam,
            whenFailedProduct: failedExam => failedExam, 
            whenCalculatedProduct: calculatedExam => calculatedExam,
            whenPublishedProduct: publishedExam => publishedExam,
            whenValidatedProduct: CalculateFinalGrade
        );

        private static IProduct CalculateFinalGrade(ValidatedProduct validProduct) =>
            new CalculatedProduct(validProduct.ProductList
                                                    .Select(CalculateCartTotalPrice)
                                                    .ToList()
                                                    .AsReadOnly());

        private static CalculatedCart CalculateCartTotalPrice(ValidatedCart validCart) => 
            new CalculatedCart(validCart.CartCod,
                                      validCart.Price,
                                      validCart.Cantity,
                                      validCart.Price * validCart.Cantity);

        public static IProduct MergeProduct(IProduct product, IEnumerable<CalculatedCart> existingProduct) => product.Match(
            whenUnvalidateProduct: unvalidaTedExam => unvalidaTedExam,
            whenInvalidProduct: invalidExam => invalidExam,
            whenFailedProduct: failedExam => failedExam,
            whenValidatedProduct: validatedExam => validatedExam,
            whenPublishedProduct: publishedExam => publishedExam,
            whenCalculatedProduct: calculatedExam => MergeProduct(calculatedExam.ProductList, existingProduct));

        private static CalculatedProduct MergeProduct(IEnumerable<CalculatedCart> newList, IEnumerable<CalculatedCart> existingList)
        {
            var updatedAndNewProduct = newList.Select(grade => grade with { ProductCod = existingList.FirstOrDefault(g => g.CartCod == grade.CartCod)?.ProductCod ?? 0, IsUpdated = true });
            var oldProduct = existingList.Where(grade => !newList.Any(g => g.CartCod == grade.CartCod));
            var allProduct = updatedAndNewProduct.Union(oldProduct)
                                               .ToList()
                                               .AsReadOnly();
            return new CalculatedProduct(allProduct);
        }

        public static IProduct PublishProduct(IProduct product) => product.Match(
            whenUnvalidateProduct: unvalidaTedExam => unvalidaTedExam,
            whenInvalidProduct: invalidExam => invalidExam,
            whenFailedProduct: failedExam => failedExam,
            whenValidatedProduct: validatedExam => validatedExam,
            whenPublishedProduct: publishedExam => publishedExam,
            whenCalculatedProduct: GenerateExport);

        private static IProduct GenerateExport(CalculatedProduct calculatedExam) => 
            new PublishedProduct(calculatedExam.ProductList, 
                                    calculatedExam.ProductList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(), 
                                    DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedCart grade) =>
            export.AppendLine($"{grade.CartCod.Value}, {grade.Price}, {grade.Cantity}, {grade.TotalPrice}");
    }
}
