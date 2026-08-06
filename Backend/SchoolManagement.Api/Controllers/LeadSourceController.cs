using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/lead-sources")]
public class LeadSourceController : ControllerBase
{
    private readonly ILeadSourceService _service;

    public LeadSourceController(ILeadSourceService service)
    {
        _service = service;
    }

    /// <summary>Get all lead sources (Ad and Opc based).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var leadSources = await _service.GetAllAsync();
        return Ok(leadSources);
    }

    /// <summary>Get a single lead source by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>Create a new Ad-based lead source.</summary>
    [HttpPost("ad")]
    public async Task<IActionResult> CreateAdLeadSource([FromBody] AdLeadSourceRequestDto dto)
    {
        try
        {
            var command = new AdLeadSourceCommand
            {
                AdId = dto.AdId
            };
            var result = await _service.CreateAdLeadSourceAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Create a new Opc-based lead source.</summary>
    [HttpPost("opc")]
    public async Task<IActionResult> CreateOpcLeadSource([FromBody] OpcLeadSourceRequestDto dto)
    {
        try
        {
            var command = new OpcLeadSourceCommand
            {
                OpcId = dto.OpcId
            };
            var result = await _service.CreateOpcLeadSourceAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Delete a lead source.</summary>
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
