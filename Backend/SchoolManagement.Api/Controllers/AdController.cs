using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/ads")]
public class AdController : ControllerBase
{
    private readonly IAdService _service;

    public AdController(IAdService service)
    {
        _service = service;
    }

    /// <summary>Get all ads (marketing campaigns on social media platforms).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ads = await _service.GetAllAsync();
        return Ok(ads);
    }

    /// <summary>Get a single ad by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>Create a new ad.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdRequestDto dto)
    {
        try
        {
            var command = new AdCommand
            {
                Name = dto.Name,
                Slug = dto.Slug,
                PlatformId = dto.PlatformId
            };
            var result = await _service.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Update an existing ad.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdRequestDto dto)
    {
        try
        {
            var command = new UpdateAdCommand
            {
                Name = dto.Name,
                Slug = dto.Slug,
                PlatformId = dto.PlatformId,
                BranchId = dto.BranchId
            };
            var result = await _service.UpdateAsync(id, command);
            return Ok(result);
        }
        catch (NotFoundException) { return NotFound(); }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Delete an ad.</summary>
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
