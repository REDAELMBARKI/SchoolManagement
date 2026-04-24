


using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Services;
using SchoolManagement.Backend.Models;
    

namespace SchoolManagement.Backend.Http.Controllers;

[ApiController]
[Route("api/intakes")]

public class IntakeController : ControllerBase
{
    private readonly IntakeService _intakeService;
    public IntakeController(IntakeService intakeService)
    {
        _intakeService = intakeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // var intakes = await _intakeService.GetAllIntakesAsync();
        // return Ok(intakes);
        var items  = new List<Intake>
        {
            new Intake
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890",
                Email = "" ,
            }
            ,
            new Intake
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "0987654321",
                Email = "" ,
            }
        } ;
        
        return Ok(items);

    }

    public async Task<IActionResult> GetById(int id)
    {
        var intake = await _intakeService.GetIntakeByIdAsync(id);
        return Ok(intake);
    }


    [HttpPost]
    public async Task<IActionResult> AddIntake(IntakeDto intake)
    {
        IntakeDto dto = new IntakeDto
        {
            FirstName = intake.FirstName,
            LastName = intake.LastName,
            Phone = intake.Phone,
            Email = intake.Email,
            IntakeDate = intake.IntakeDate,
            LeadSourceId = intake.LeadSourceId,
            GenderId = intake.GenderId,

        };
        await _intakeService.AddIntakeAsync(dto);
        return NoContent();
    }

    public async Task<IActionResult> Update(int id, Intake intake)
    {
        if (id != intake.Id)
        {
            return BadRequest();
        }

        return NoContent();
    }

    public async Task<IActionResult> Delete(int id)
    {
        var intake = await _intakeService.GetIntakeByIdAsync(id);
        if (intake == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}