using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefundController : ControllerBase
{
    private readonly IRefundService _refundService;

    public RefundController(IRefundService refundService)
    {
        _refundService = refundService;
    }

    /// <summary>
    /// Records a refund against a payment.
    /// Reduces the linked invoice's PaidAmount and recalculates status.
    /// </summary>
    [HttpPost("payment/{paymentId}")]
    public async Task<IActionResult> RefundPayment(Guid paymentId, [FromBody] RefundCommand command)
    {
        try
        {
            var result = await _refundService.RefundPaymentAsync(paymentId, command);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Refund error",
                detail: ex.Message
            );
        }
    }

    /// <summary>
    /// Get all refunds for a specific payment.
    /// </summary>
    [HttpGet("payment/{paymentId}")]
    public async Task<IActionResult> GetByPayment(Guid paymentId)
    {
        try
        {
            var refunds = await _refundService.GetByPaymentIdAsync(paymentId);
            return Ok(refunds);
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Fetch refunds error",
                detail: ex.Message
            );
        }
    }
}
