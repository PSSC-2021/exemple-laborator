using Exemple.Domain.Models;
using LanguageExt;
using System.Collections.Generic;
using static Exemple.Domain.Models.Product;

namespace Exemple.Domain.Repositories
{
    public interface IProductRepository
    {
        TryAsync<List<CalculatedCart>> TryGetExistingProduct();

        TryAsync<Unit> TrySaveProduct(PublishedProduct product);
    }
}
