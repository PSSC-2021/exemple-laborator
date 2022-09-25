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
                          join s in dbContext.Students on g.StudentId equals s.StudentId
                          select new { s.RegistrationNumber, g.GradeId, g.Exam, g.Activity, g.Final })
                          .AsNoTracking()
                          .ToListAsync())
                          .Select(result => new CalculatedSudentGrade(
                                                    StudentRegistrationNumber: new(result.RegistrationNumber),
                                                    ExamGrade: new(result.Exam ?? 0m),
                                                    ActivityGrade: new(result.Activity ?? 0m),
                                                    FinalGrade: new(result.Final ?? 0m))
                          { 
                            GradeId = result.GradeId
                          })
                          .ToList();

        public TryAsync<Unit> TrySaveGrades(PublishedExamGrades grades) => async () =>
        {
            var students = (await dbContext.Students.ToListAsync()).ToLookup(student=>student.RegistrationNumber);
            var newGrades = grades.GradeList
                                    .Where(g => g.IsUpdated && g.GradeId == 0)
                                    .Select(g => new GradeDto()
                                    {
                                        StudentId = students[g.StudentRegistrationNumber.Value].Single().StudentId,
                                        Exam = g.ExamGrade.Value,
                                        Activity = g.ActivityGrade.Value,
                                        Final = g.FinalGrade.Value,
                                    });
            var updatedGrades = grades.GradeList.Where(g => g.IsUpdated && g.GradeId > 0)
                                    .Select(g => new GradeDto()
                                    {
                                        GradeId = g.GradeId,
                                        StudentId = students[g.StudentRegistrationNumber.Value].Single().StudentId,
                                        Exam = g.ExamGrade.Value,
                                        Activity = g.ActivityGrade.Value,
                                        Final = g.FinalGrade.Value,
                                    });

            dbContext.AddRange(newGrades);
            foreach (var entity in updatedGrades)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return unit;
        };
    }
}
