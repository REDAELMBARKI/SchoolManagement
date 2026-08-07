using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollPaymentController : ControllerBase
{
    private readonly IPayrollPaymentService _payrollService;

    public PayrollPaymentController(IPayrollPaymentService payrollService)
    {
        _payrollService = payrollService;
    }

    /// <summary>
    /// Get all payroll payments.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payrolls = await _payrollService.GetAllAsync();
        return Ok(payrolls);
    }

    /// <summary>
    /// Get payroll payment by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var payroll = await _payrollService.GetByIdAsync(id);
            return Ok(payroll);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get all payroll payments for a specific employee.
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId)
    {
        var payrolls = await _payrollService.GetByEmployeeIdAsync(employeeId);
        return Ok(payrolls);
    }

    /// <summary>
    /// Get payroll payments for a specific period (e.g., ?year=2026&month=8).
    /// </summary>
    [HttpGet("period")]
    public async Task<IActionResult> GetByPeriod([FromQuery] int year, [FromQuery] int month)
    {
        if (year < 2000 || month < 1 || month > 12)
            return BadRequest("Invalid year or month.");

        var payrolls = await _payrollService.GetByPeriodAsync(year, month);
        return Ok(payrolls);
    }

    /// <summary>
    /// Create a new payroll payment (pending status).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PayrollPaymentCommand command)
    {
        try
        {
            var payroll = await _payrollService.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = payroll.Id }, payroll);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mark a payroll payment as paid.
    /// </summary>
    [HttpPost("{id}/mark-paid")]
    public async Task<IActionResult> MarkAsPaid(Guid id, [FromBody] MarkPayrollPaidCommand command)
    {
        try
        {
            var payroll = await _payrollService.MarkAsPaidAsync(id, command);
            return Ok(payroll);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a payroll payment (only if not yet paid).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _payrollService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
