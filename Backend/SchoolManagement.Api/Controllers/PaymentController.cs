using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IRefundService _refundService;

    public PaymentController(IPaymentService paymentService, IRefundService refundService)
    {
        _paymentService = paymentService;
        _refundService = refundService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _paymentService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _paymentService.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>Record a registration payment (cash at counter).</summary>
    [HttpPost("registration")]
    public async Task<IActionResult> CreateRegistrationPayment([FromBody] RegistrationPaymentRequestDto dto)
    {
        try
        {
            var command = new RegistrationPaymentCommand
            {
                EnrollmentId = dto.EnrollmentId,
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                TransferFees = dto.TransferFees,
                Method = dto.Method,
                PaidAt = dto.PaidAt,
                ExternalReferenceCode = dto.ExternalReferenceCode,
                MethodDetailsJson = dto.MethodDetailsJson ?? "{}"
            };
            var result = await _paymentService.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Settle a charge via invoice payment.</summary>
    [HttpPost("settle")]
    public async Task<IActionResult> SettleCharge([FromBody] ChargeSettlementPaymentRequestDto dto)
    {
        try
        {
            var command = new ChargeSettlementPaymentCommand
            {
                EnrollmentId = dto.EnrollmentId,
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                TransferFees = dto.TransferFees,
                Method = dto.Method,
                PaidAt = dto.PaidAt,
                ExternalReferenceCode = dto.ExternalReferenceCode,
                MethodDetailsJson = dto.MethodDetailsJson ?? "{}"
            };
            var result = await _paymentService.SettleChargeAsync(command);
            return Ok(result);
        }
        catch (NotFoundException) { return NotFound(); }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>
    /// Issue a cash refund against a payment.
    /// Refunds are always cash — the member comes in person to collect.
    /// Partial refunds are supported; multiple refunds on the same payment are allowed.
    /// </summary>
    [HttpPost("{id}/refund")]
    public async Task<IActionResult> Refund(Guid id, [FromBody] RefundCommand command)
    {
        try
        {
            var result = await _refundService.RefundPaymentAsync(id, command);
            return Ok(result);
        }
        catch (NotFoundException) { return NotFound(); }
        catch (DomainException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return Problem(detail: ex.Message, statusCode: 500); }
    }

    /// <summary>Get all refunds for a specific payment.</summary>
    [HttpGet("{id}/refunds")]
    public async Task<IActionResult> GetRefunds(Guid id)
    {
        var result = await _refundService.GetByPaymentIdAsync(id);
        return Ok(result);
    }
}
