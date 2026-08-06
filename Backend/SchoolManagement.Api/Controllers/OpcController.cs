using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/opcs")]
public class OpcController : ControllerBase
{
    private readonly IOpcService _service;

    public OpcController(IOpcService service)
    {
        _service = service;
    }

    /// <summary>Get all OPCs (phone/in-person lead handlers).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var opcs = await _service.GetAllAsync();
        return Ok(opcs);
    }

    /// <summary>Get a single OPC by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>Register a new OPC.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OpcRequestDto dto)
    {
        try
        {
            var command = new OpcCommand
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Slug = dto.Slug,
                GenderId = dto.GenderId,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                HireDate = dto.HireDate ?? DateTime.UtcNow,
                Salary = dto.Salary
            };
            var result = await _service.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Update an existing OPC.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOpcRequestDto dto)
    {
        try
        {
            var command = new UpdateOpcCommand
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Slug = dto.Slug,
                GenderId = dto.GenderId,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                HireDate = dto.HireDate,
                Salary = dto.Salary,
                BranchId = dto.BranchId
            };
            var result = await _service.UpdateAsync(id, command);
            return Ok(result);
        }
        catch (NotFoundException) { return NotFound(); }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Delete an OPC.</summary>
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
