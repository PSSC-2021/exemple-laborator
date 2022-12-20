using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using Example.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static Exemple.Domain.Models.ExamGrades;
using static LanguageExt.Prelude;

namespace Example.Data.Repositories
{
    public class GradesRepository : IGradesRepository
    {
        private readonly GradesContext dbContext;

        public GradesRepository(GradesContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TryAsync<List<CalculatedSudentGrade>> TryGetExistingGrades() => async () => (await (
                          from g in dbContext.Grades
                          join s in dbContext.Students on g.ProductId equals s.ProductId
                          select new { s.Name, g.ProductId, g.Quantity, g.Subtotal, g.Total })
                          .AsNoTracking()
                          .ToListAsync())
                          .Select(result => new CalculatedSudentGrade(
                                                    Name: new(result.Name),
                                                    Quantity: new(result.Quantity ?? 0m),
                                                    Subtotal: new(result.Subtotal ?? 0m),
                                                    Total: new(result.Total ?? 0m))
                          { 
                            CommandId = result.ProductId
                          })
                          .ToList();

        public TryAsync<Unit> TrySaveGrades(PublishedExamGrades grades) => async () =>
        {
            var students = (await dbContext.Students.ToListAsync()).ToLookup(student=>student.Name);
            var newGrades = grades.GradeList
                                    //.Where(g => g.IsUpdated && g.CommandId == 0)
                                    .Select(g => new GradeDto()
                                    {
                                        ProductId = students[g.Name.Value].Single().ProductId,
                                        Quantity = g.Quantity.Value,
                                        Subtotal = g.Subtotal.Value,
                                        Total = g.Total.Value,
                                    });
            // var updatedGrades = grades.GradeList.Where(g => g.IsUpdated && g.CommandId > 0)
            //                         .Select(g => new GradeDto()
            //                         {
            //                             CommandId = g.CommandId,
            //                             ProductId = students[g.Name.Value].Single().ProductId,
            //                             Quantity = g.Quantity.Value,
            //                             Subtotal = g.Subtotal.Value,
            //                             Total = g.Total.Value,
            //                         });

            dbContext.AddRange(newGrades);
            // foreach (var entity in updatedGrades)
            // {
            //     dbContext.Entry(entity).State = EntityState.Modified;
            // }

            await dbContext.SaveChangesAsync();

            return unit;
        };
    }
}
