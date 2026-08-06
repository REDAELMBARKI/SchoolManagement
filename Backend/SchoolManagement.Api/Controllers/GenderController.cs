using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/genders")]
public class GenderController : ControllerBase
{
    private readonly IGenderService _service;

    public GenderController(IGenderService service)
    {
        _service = service;
    }

    /// <summary>Get all genders.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var genders = await _service.GetAllAsync();
        return Ok(genders);
    }

    /// <summary>Get a single gender by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>Create a new gender.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GenderRequestDto dto)
    {
        try
        {
            var command = new GenderCommand
            {
                Name = dto.Name,
                Slug = dto.Slug
            };
            var result = await _service.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Update an existing gender.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGenderRequestDto dto)
    {
        try
        {
            var command = new UpdateGenderCommand
            {
                Name = dto.Name,
                Slug = dto.Slug
            };
            var result = await _service.UpdateAsync(id, command);
            return Ok(result);
        }
        catch (NotFoundException) { return NotFound(); }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Delete a gender.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException) { return NotFound(); }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }
}
