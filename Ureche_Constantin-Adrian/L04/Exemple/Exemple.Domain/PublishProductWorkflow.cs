using Exemple.Domain.Models;
using static Exemple.Domain.Models.ProductPublishedEvent;
using static Exemple.Domain.ProductOperation;
using System;
using static Exemple.Domain.Models.Product;
using LanguageExt;
using System.Threading.Tasks;
using System.Collections.Generic;
using Exemple.Domain.Repositories;
using System.Linq;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;

namespace Exemple.Domain
{
    public class PublishProductWorkflow
    {
        private readonly ICartRepository studentsRepository;
        private readonly IProductRepository productRepository;
        private readonly ILogger<PublishProductWorkflow> logger;

        public PublishProductWorkflow(ICartRepository studentsRepository, IProductRepository productRepository, ILogger<PublishProductWorkflow> logger)
        {
            this.studentsRepository = studentsRepository;
            this.productRepository = productRepository;
            this.logger = logger;
        }

        public async Task<IProductPublishedEvent> ExecuteAsync(PublishProductCommand command)
        {
            UnvalidateProduct unvalidatedProduct = new UnvalidateProduct(command.InputProduct);

            var result = from students in studentsRepository.TryGetExistingCart(unvalidatedProduct.ProductList.Select(grade => grade.CartCod))
                                          .ToEither(ex => new FailedProduct(unvalidatedProduct.ProductList, ex) as IProduct)
                         from existingProduct in productRepository.TryGetExistingProduct()
                                          .ToEither(ex => new FailedProduct(unvalidatedProduct.ProductList, ex) as IProduct)
                         let checkStudentExists = (Func<CartCod, Option<CartCod>>)(student => CheckCartExists(students, student))
                         from publishedProduct in ExecuteWorkflowAsync(unvalidatedProduct, existingProduct, checkStudentExists).ToAsync()
                         from _ in productRepository.TrySaveProduct(publishedProduct)
                                          .ToEither(ex => new FailedProduct(unvalidatedProduct.ProductList, ex) as IProduct)
                         select publishedProduct;

            return await result.Match(
                    Left: product => GenerateFailedEvent(product) as IProductPublishedEvent,
                    Right: publishedProduct => new ProductPublishScucceededEvent(publishedProduct.Csv, publishedProduct.PublishedDate)
                );
        }

        private async Task<Either<IProduct, PublishedProduct>> ExecuteWorkflowAsync(UnvalidateProduct unvalidatedProduct, 
                                                                                          IEnumerable<CalculatedCart> existingProduct, 
                                                                                          Func<CartCod, Option<CartCod>> checkStudentExists)
        {
            
            IProduct product = await ValidateProduct(checkStudentExists, unvalidatedProduct);
            product = CalculateFinalProduct(product);
            product = MergeProduct(product, existingProduct);
            product = PublishProduct(product);

            return product.Match<Either<IProduct, PublishedProduct>>(
                whenUnvalidateProduct: unvalidatedProduct => Left(unvalidatedProduct as IProduct),
                whenCalculatedProduct: calculatedProduct => Left(calculatedProduct as IProduct),
                whenInvalidProduct: invalidProduct => Left(invalidProduct as IProduct),
                whenFailedProduct: failedProduct => Left(failedProduct as IProduct),
                whenValidatedProduct: validatedProduct => Left(validatedProduct as IProduct),
                whenPublishedProduct: publishedProduct => Right(publishedProduct)
            );
        }

        private Option<CartCod> CheckCartExists(IEnumerable<CartCod> students, CartCod studentRegistrationNumber)
        {
            if(students.Any(s=>s == studentRegistrationNumber))
            {
                return Some(studentRegistrationNumber);
            }
            else
            {
                return None;
            }
        }

        private ProductPublishFaildEvent GenerateFailedEvent(IProduct product) =>
            product.Match<ProductPublishFaildEvent>(
                whenUnvalidateProduct: UnvalidateProduct => new($"Invalid state {nameof(UnvalidateProduct)}"),
                whenInvalidProduct: InvalidProduct => new(InvalidProduct.Reason),
                whenValidatedProduct: ValidatedProduct => new($"Invalid state {nameof(ValidatedProduct)}"),
                whenFailedProduct: FailedProduct =>
                {
                    logger.LogError(FailedProduct.Exception, FailedProduct.Exception.Message);
                    return new(FailedProduct.Exception.Message);
                },
                whenCalculatedProduct: CalculatedProduct => new($"Invalid state {nameof(CalculatedProduct)}"),
                whenPublishedProduct: PublishedProduct => new($"Invalid state {nameof(PublishedProduct)}"));
    }
}
