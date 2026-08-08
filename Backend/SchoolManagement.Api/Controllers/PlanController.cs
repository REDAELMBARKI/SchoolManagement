using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanController : ControllerBase
{
    private readonly IPlanService _planService;

    public PlanController(IPlanService planService)
    {
        _planService = planService;
    }

    /// <summary>
    /// Get all payment plans (active and inactive).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _planService.GetAllAsync();
        return Ok(plans);
    }

    /// <summary>
    /// Get only active payment plans.
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var plans = await _planService.GetActiveAsync();
        return Ok(plans);
    }

    /// <summary>
    /// Get a specific plan by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var plan = await _planService.GetByIdAsync(id);
            return Ok(plan);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Create a new payment plan.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanRequestDto request)
    {
        try
        {
            var command = new PlanCommand
            {
                Name = request.Name,
                DurationMonths = request.DurationMonths,
                BaseAmount = request.BaseAmount,
                DiscountPercent = request.DiscountPercent,
                IsActive = request.IsActive,
                RemainingAmountDueDays = request.RemainingAmountDueDays
            };

            var plan = await _planService.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing payment plan.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanRequestDto request)
    {
        try
        {
            var command = new UpdatePlanCommand
            {
                Name = request.Name,
                DurationMonths = request.DurationMonths,
                BaseAmount = request.BaseAmount,
                DiscountPercent = request.DiscountPercent,
                IsActive = request.IsActive,
                RemainingAmountDueDays = request.RemainingAmountDueDays
            };

            var plan = await _planService.UpdateAsync(id, command);
            return Ok(plan);
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
    /// Delete a payment plan.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _planService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
