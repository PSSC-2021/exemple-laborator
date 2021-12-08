using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Example.Data.Repositories
{
    public class CartRepository: ICartRepository
    {
        private readonly ProductContext productContext;

        public CartRepository(ProductContext productContext)
        {
            this.productContext = productContext;  
        }

        public TryAsync<List<CartCod>> TryGetExistingCart(IEnumerable<string> studentsToCheck) => async () =>
        {
            var students = await productContext.Students
                                              .Where(student => studentsToCheck.Contains(student.RegistrationNumber))
                                              .AsNoTracking()
                                              .ToListAsync();
            return students.Select(student => new CartCod(student.RegistrationNumber))
                           .ToList();
        };
    }
}
