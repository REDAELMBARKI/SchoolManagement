


using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Services;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Application.Interfaces.Services;


namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/intakes")]
public class IntakeController : ControllerBase
{


    private readonly IIntakeService _intakeService;
    public IntakeController(IIntakeService intakeService)
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
    public async Task<IActionResult> GetById(Guid id)
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
    public async Task<IActionResult> Add(IntakeRequestDto intakeDto)
    {
        var newIntake = await _intakeService.AddIntakeAsync(intakeDto);
        return CreatedAtAction(nameof(GetById), new { id = newIntake.Id }, newIntake);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, IntakeRequestDto intake)
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
    public async Task<IActionResult> Delete(Guid id)
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
             detail : "Failed to delete intake"
            );
        }
    }










}