using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Dtos.Requests;

namespace SchoolManagement.Backend.Api.Controllers
{

    [ApiController]
    [Route("api/students/register")]
    public class StudentRegistrationController : ControllerBase
    {

        public StudentRegistrationController() { }


        [HttpPost]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRequestDto request)
        {
            awa
        }
}
