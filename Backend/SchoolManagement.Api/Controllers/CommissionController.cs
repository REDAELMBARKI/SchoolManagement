using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Enums;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/commissions")]
public class CommissionController : ControllerBase
{
    private readonly ICommissionService _commissionService;

    public CommissionController(ICommissionService commissionService)
    {
        _commissionService = commissionService;
    }

    /// <summary>Get all commissions for a specific earner (OPC or Agent).</summary>
    [HttpGet("earner/{earnerId}")]
    public async Task<IActionResult> GetByEarner(Guid earnerId, [FromQuery] EarnerType earnerType)
    {
        var result = await _commissionService.GetByEarnerAsync(earnerId, earnerType);
        return Ok(result);
    }

    /// <summary>Get all commissions for a given month (e.g. ?year=2026&amp;month=8).</summary>
    [HttpGet("period")]
    public async Task<IActionResult> GetByPeriod([FromQuery] int year, [FromQuery] int month)
    {
        if (year < 2000 || month < 1 || month > 12)
            return BadRequest("Invalid year or month.");

        var result = await _commissionService.GetByPeriodAsync(year, month);
        return Ok(result);
    }

    /// <summary>
    /// Manually block a commission (manager decision).
    /// Only allowed before the salary lockout date.
    /// </summary>
    [HttpPost("{id}/block")]
    public async Task<IActionResult> Block(Guid id, [FromBody] BlockCommissionRequestDto dto)
    {
        try
        {
            var result = await _commissionService.BlockCommissionAsync(id, dto.Reason);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
