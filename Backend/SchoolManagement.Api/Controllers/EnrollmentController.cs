using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enrollments = await _enrollmentService.GetAllAsync();
        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);
            return Ok(enrollment);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Fetch error",
                detail: ex.Message
            );
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EnrollmentRequestDto dto)
    {
        try
        {
            var command = new EnrollmentCommand
            {
                LevelId = dto.LevelId,
                SubjectId = dto.SubjectId,
                Notes = dto.Notes,
                PreferedScheduleId = dto.PreferedScheduleId,
                StudentId = dto.StudentId,
                GroupId = dto.GroupId ?? Guid.Empty
            };
            var enrollment = await _enrollmentService.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEnrollmentRequestDto dto)
    {
        try
        {
            var command = new UpdateEnrollmentCommand
            {
                PreferedScheduleId = dto.PreferedScheduleId,
                LevelId = dto.LevelId,
                StudentId = dto.StudentId,
                SubjectId = dto.SubjectId,
                Notes = dto.Notes,
                GroupId = dto.GroupId
            };
            await _enrollmentService.UpdateAsync(id, command);
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
                title: "Update error",
                detail: ex.Message
            );
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _enrollmentService.DeleteAsync(id);
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
                title: "Delete error",
                detail: ex.Message
            );
        }
    }

    [HttpPost("{id}/drop")]
    public async Task<IActionResult> Drop(Guid id, [FromBody] DropEnrollmentRequestDto dto)
    {
        try
        {
            var command = new DropEnrollmentCommand
            {
                EnrollmentId = id,
                Reason = dto.Reason
            };

            var enrollment = await _enrollmentService.DropEnrollmentAsync(command);
            return Ok(enrollment);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Drop error",
                detail: ex.Message
            );
        }
    }

}
