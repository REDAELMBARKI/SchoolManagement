using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/teachers")]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _service;

    public TeacherController(ITeacherService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TeacherCommand command)
    {
        try
        {
            var result = await _service.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherCommand command)
    {
        try
        {
            var result = await _service.UpdateAsync(id, command);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }
}
