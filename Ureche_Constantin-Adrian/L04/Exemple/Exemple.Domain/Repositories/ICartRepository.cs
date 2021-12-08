using Exemple.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Repositories
{
    public interface ICartRepository
    {
        TryAsync<List<CartCod>> TryGetExistingCart(IEnumerable<string> studentsToCheck);
    }
}
