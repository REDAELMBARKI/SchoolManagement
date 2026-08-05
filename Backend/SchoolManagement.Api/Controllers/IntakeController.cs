using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;


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

        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "fetch error",
                detail: ex.Message
            );
        }
    }





    [HttpPost]
    public async Task<IActionResult> Add(IntakeRequestDto dto)
    {
        try
        {
            var command = new IntakeCommand
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                GenderId = dto.GenderId,
                IntakeDate = dto.IntakeDate,
                Status = dto.Status,
                FollowUpDate = dto.FollowUpDate,
                Notes = dto.Notes,
                CommercialAgentId = dto.CommercialAgentId,
                LeadSourceId = dto.IsIndependent ? null : dto.LeadSource?.SourceId,
                SubjectId = dto.SubjectId,
                IsIndependent = dto.IsIndependent,
                TotalFees = dto.TotalFees,
                AmountPaid = dto.AmountPaid
            };
            var newIntake = await _intakeService.AddIntakeAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = newIntake.Id }, newIntake);
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Create error",
                detail: ex.Message
            );
        }
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, IntakeRequestDto dto)
    {
        try
        {
            var command = new UpdateIntakeCommand
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                GenderId = dto.GenderId,
                IntakeDate = dto.IntakeDate,
                Status = dto.Status,
                FollowUpDate = dto.FollowUpDate,
                Notes = dto.Notes,
                CommercialAgentId = dto.CommercialAgentId,
                LeadSourceId = dto.IsIndependent ? null : dto.LeadSource?.SourceId,
                SubjectId = dto.SubjectId,
                IsIndependent = dto.IsIndependent,
                TotalFees = dto.TotalFees,
                AmountPaid = dto.AmountPaid
            };
            await _intakeService.UpdateAsync(id, command);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "update error",
                detail: ex.Message
            );
        }
    }







    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _intakeService.DeleteIntakeAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return Problem(
             statusCode: 500,
             title: "Delete Error",
             detail: "Failed to delete intake"
            );
        }
    }











}
