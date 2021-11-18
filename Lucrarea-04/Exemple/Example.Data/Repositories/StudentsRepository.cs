using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Example.Data.Repositories
{
    public class StudentsRepository: IStudentsRepository
    {
        private readonly GradesContext gradesContext;

        public StudentsRepository(GradesContext gradesContext)
        {
            this.gradesContext = gradesContext;  
        }

        public TryAsync<List<StudentRegistrationNumber>> TryGetExistingStudents(IEnumerable<string> studentsToCheck) => async () =>
        {
            var students = await gradesContext.Students
                                              .Where(student => studentsToCheck.Contains(student.RegistrationNumber))
                                              .AsNoTracking()
                                              .ToListAsync();
            return students.Select(student => new StudentRegistrationNumber(student.RegistrationNumber))
                           .ToList();
        };
    }
}
