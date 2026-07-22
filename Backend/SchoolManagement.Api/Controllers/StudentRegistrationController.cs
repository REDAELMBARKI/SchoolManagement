using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Services.Registrations;

namespace SchoolManagement.Backend.Api.Controllers
{

    [ApiController]
    [Route("api/students/register")]
    public class StudentRegistrationController : ControllerBase
    {

        private readonly StudentRegistrationService _studentRegistrationService;

        public StudentRegistrationController(StudentRegistrationService studentRegistrationService)
        {
            _studentRegistrationService = studentRegistrationService;
        }


        [HttpPost]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegistrationRequestDto request)
        {
            var StudentRegistrationResult = await _studentRegistrationService.RegisterStudentAsync(request);
            return Ok(StudentRegistrationResult);
        }
}
