using Exemple.Domain;
using Exemple.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using Example.Api.Models;
using Exemple.Domain.Models;

namespace Example.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private ILogger<ProductsController> logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromServices] IProductRepository productRepository) =>
            await productRepository.TryGetExistingProduct().Match(
               Succ: GetAllProductHandleSuccess,
               Fail: GetAllProductHandleError
            );

        private ObjectResult GetAllProductHandleError(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllProductHandleSuccess(List<Exemple.Domain.Models.CalculatedCart> products) =>
        Ok(products.Select(product => new
        {
            CartCod = product.CartCod.Value,
            product.Price,
            product.Cantity,
            product.TotalPrice
        }));

        [HttpPost]
        public async Task<IActionResult> PublishProducts([FromServices]PublishProductWorkflow publishGradeWorkflow, [FromBody]InputProduct[] product)
        {
            var unvalidatedProducts = product.Select(MapInputProductToUnvalidatedProduct)
                                          .ToList()
                                          .AsReadOnly();
            PublishProductCommand command = new(unvalidatedProducts);
            var result = await publishGradeWorkflow.ExecuteAsync(command);
            return result.Match<IActionResult>(
                whenProductPublishFaildEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
                whenProductPublishScucceededEvent: successEvent => Ok()
            );
        }

        private static UnvalidatedCart MapInputProductToUnvalidatedProduct(InputProduct grade) => new UnvalidatedCart(
            CartCod: grade.Cod,
            Price: grade.Price.ToString(),
            Cantity: grade.Cantity.ToString());
    }
}
