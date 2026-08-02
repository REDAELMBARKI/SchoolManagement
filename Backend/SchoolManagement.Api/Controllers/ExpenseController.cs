using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _service;

    public ExpenseController(IExpenseService service)
    {
        _service = service;
    }

    /// <summary>Get all expenses (optionally filtered).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? branchId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] ExpenseType? category,
        [FromQuery] Guid? staffId)
    {
        if (branchId == null && startDate == null && endDate == null
            && category == null && staffId == null)
        {
            var all = await _service.GetAllAsync();
            return Ok(all);
        }

        var filtered = await _service.GetFilteredAsync(branchId, startDate, endDate, category, staffId);
        return Ok(filtered);
    }

    /// <summary>Get a single expense by ID.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>Record a new cash outflow expense.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExpenseRequestDto dto)
    {
        try
        {
            var command = new ExpenseCommand
            {
                Category = dto.Category,
                PayeeName = dto.PayeeName,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                PaymentMethod = dto.PaymentMethod,
                Description = dto.Description,
                Reference = dto.Reference
            };
            var result = await _service.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Update an existing expense record.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseRequestDto dto)
    {
        try
        {
            var command = new UpdateExpenseCommand
            {
                Category = dto.Category,
                PayeeName = dto.PayeeName,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                PaymentMethod = dto.PaymentMethod,
                Description = dto.Description,
                Reference = dto.Reference
            };
            var result = await _service.UpdateAsync(id, command);
            return Ok(result);
        }
        catch (NotFoundException) { return NotFound(); }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Soft-delete an expense record.</summary>
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
