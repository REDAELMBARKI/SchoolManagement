using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Services.Registrations;

namespace SchoolManagement.Api.Controllers;

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
