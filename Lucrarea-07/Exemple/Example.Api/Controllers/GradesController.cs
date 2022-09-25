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
    public class GradesController : ControllerBase
    {
        private ILogger<GradesController> logger;

        public GradesController(ILogger<GradesController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrades([FromServices] IGradesRepository gradesRepository) =>
            await gradesRepository.TryGetExistingGrades().Match(
               Succ: GetAllGradesHandleSuccess,
               Fail: GetAllGradesHandleError
            );

        private ObjectResult GetAllGradesHandleError(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllGradesHandleSuccess(List<Exemple.Domain.Models.CalculatedSudentGrade> grades) =>
        Ok(grades.Select(grade => new
        {
            StudentRegistrationNumber = grade.StudentRegistrationNumber.Value,
            grade.ExamGrade,
            grade.ActivityGrade,
            grade.FinalGrade
        }));

        [HttpPost]
        public async Task<IActionResult> PublishGrades([FromServices]PublishGradeWorkflow publishGradeWorkflow, [FromBody]InputGrade[] grades)
        {
            var unvalidatedGrades = grades.Select(MapInputGradeToUnvalidatedGrade)
                                          .ToList()
                                          .AsReadOnly();
            PublishGradesCommand command = new(unvalidatedGrades);
            var result = await publishGradeWorkflow.ExecuteAsync(command);
            return result.Match<IActionResult>(
                whenExamGradesPublishFaildEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
                whenExamGradesPublishScucceededEvent: successEvent => Ok()
            );
        }

        private static UnvalidatedStudentGrade MapInputGradeToUnvalidatedGrade(InputGrade grade) => new UnvalidatedStudentGrade(
            StudentRegistrationNumber: grade.RegistrationNumber,
            ExamGrade: grade.Exam,
            ActivityGrade: grade.Activity);
    }
}
