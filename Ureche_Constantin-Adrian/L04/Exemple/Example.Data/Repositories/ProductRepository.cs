using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using Example.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static Exemple.Domain.Models.Product;
using static LanguageExt.Prelude;

namespace Example.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext dbContext;

        public ProductRepository(ProductContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TryAsync<List<CalculatedCart>> TryGetExistingProduct() => async () => (await (
                          from g in dbContext.Product
                          join s in dbContext.Students on g.CartCod equals s.CartCod
                          select new { s.RegistrationNumber, g.ProductCod, g.Price, g.Cantity, g.Total })
                          .AsNoTracking()
                          .ToListAsync())
                          .Select(result => new CalculatedCart(
                                                    CartCod: new(result.RegistrationNumber),
                                                    Price: new(result.Price ?? 0m),
                                                    Cantity: new(result.Cantity ?? 0m),
                                                    TotalPrice: new(result.Total ?? 0m))
                          { 
                            ProductCod = result.ProductCod
                          })
                          .ToList();

        public TryAsync<Unit> TrySaveProduct(PublishedProduct product) => async () =>
        {
            var students = (await dbContext.Students.ToListAsync()).ToLookup(student=>student.RegistrationNumber);
            var newProduct = product.ProductList
                                    .Where(g => g.IsUpdated && g.ProductCod == 0)
                                    .Select(g => new ProductDto()
                                    {
                                        CartCod = students[g.CartCod.Value].Single().CartCod,
                                        Price = g.Price.Value,
                                        Cantity = g.Cantity.Value,
                                        Total = g.TotalPrice.Value,
                                    });
            var updatedProduct = product.ProductList.Where(g => g.IsUpdated && g.ProductCod > 0)
                                    .Select(g => new ProductDto()
                                    {
                                        ProductCod = g.ProductCod,
                                        CartCod = students[g.CartCod.Value].Single().CartCod,
                                        Price = g.Price.Value,
                                        Cantity = g.Cantity.Value,
                                        Total = g.TotalPrice.Value,
                                    });

            dbContext.AddRange(newProduct);
            foreach (var entity in updatedProduct)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return unit;
        };
    }
}
