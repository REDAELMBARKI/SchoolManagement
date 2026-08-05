using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;
namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var student = await _studentService.GetByIdAsync(id);
            return Ok(student);
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
    public async Task<IActionResult> Create([FromBody] StudentRequestDto dto)
    {
        try
        {
            var command = new StudentCommand
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                GenderId = dto.GenderId,
                LevelId = dto.LevelId,
                IntakeId = dto.IntakeId,
                IsDirectRegistration = dto.IsDirectRegistration,
            };
            var student = await _studentService.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
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
    public async Task<IActionResult> Update(Guid id, [FromBody] StudentRequestDto dto)
    {
        try
        {
            var command = new UpdateStudentCommand
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                GenderId = dto.GenderId,
                IntakeId = dto.IntakeId,
                IsDirectRegistration = dto.IsDirectRegistration
            };
            await _studentService.UpdateAsync(id, command);
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
            await _studentService.DeleteAsync(id);
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
}
