


using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.DTOs;
using SchoolManagement.Http.Requests;
using SchoolManagement.Services;
namespace SchoolManagement.backend.Http.Controllers;

[ApiController]
[Route("api/intakes")]

public class  IntakeController : ControllerBase
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
        return NoContent() ;
    }

    public async Task<IActionResult> GetById(int id)
    {
         var intake = await _intakeService.GetIntakeByIdAsync(id);
         return Ok(intake);
    }

    public async Task<IActionResult> Create(IntakeRequest intake)
    {
        IntakeDTO dto = new IntakeDTO
        {
            FirstName = intake.FirstName,
            LastName = intake.LastName,
            Phone = intake.Phone,
            Email = intake.Email,
            IntakeDate = intake.IntakeDate,
            OpcId = intake.OpcId,
            LeadSourceId = intake.LeadSourceId,
            GenderId = intake.GenderId,
            
        };
        
        await _intakeService.AddIntakeAsync(dto);
        return NoContent() ;
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