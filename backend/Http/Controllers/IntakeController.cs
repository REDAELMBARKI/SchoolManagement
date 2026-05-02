


using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Services;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Exceptions;
using Microsoft.Extensions.FileProviders;


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
        var intakes = await _intakeService.GetAllIntakesAsync();
        return Ok(intakes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
           var intake = await _intakeService.GetIntakeByIdAsync(id);
           return Ok(intake);
            
        }catch(NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode : 500 ,
                title : "fetch error" ,
                detail : ex.Message
            ) ;
        }
    }


    [HttpPost]
    public async Task<IActionResult> Add(IntakeDto intakeDto)
    {
        var newIntake = await _intakeService.AddIntakeAsync(intakeDto);
        return CreatedAtAction(nameof(GetById), new { id = newIntake.Id }, newIntake);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, IntakeDto intake)
    {
        try
        {
            await _intakeService.UpdateAsync(id , intake);
            return NoContent();
        }catch(NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode : 500 ,
                title : "update error" ,
                detail : ex.Message
            ) ;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _intakeService.DeleteIntakeAsync(id);
            return NoContent();
        }catch(NotFoundException)
        {
            return NotFound() ;
        }
        catch (Exception)
        {
            return Problem(
             statusCode : 500 ,
             title : "Delete Error" ,
             detail : "field to delete intake"
            );
        }
    }
}