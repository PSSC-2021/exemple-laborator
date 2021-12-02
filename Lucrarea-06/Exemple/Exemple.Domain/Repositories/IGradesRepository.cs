using Exemple.Domain.Models;
using LanguageExt;
using System.Collections.Generic;
using static Exemple.Domain.Models.ExamGrades;

namespace Exemple.Domain.Repositories
{
    public interface IGradesRepository
    {
        TryAsync<List<CalculatedSudentGrade>> TryGetExistingGrades();

        TryAsync<Unit> TrySaveGrades(PublishedExamGrades grades);
    }
}
