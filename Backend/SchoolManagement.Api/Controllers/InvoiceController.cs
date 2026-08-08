using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return Ok(invoices);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequestDto request)
    {
        var command = new InvoiceCommand
        {
            EnrollmentId = request.EnrollmentId,
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            DueDate = request.DueDate,
            Charge = request.Charge
        };

        var result = await _invoiceService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceRequestDto request)
    {
        var command = new UpdateInvoiceCommand
        {
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            DueDate = request.DueDate
        };

        var result = await _invoiceService.UpdateAsync(id, command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _invoiceService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/waive")]
    public async Task<IActionResult> Waive(Guid id, [FromBody] WaiveInvoiceRequestDto request)
    {
        var command = new WaiveInvoiceCommand
        {
            InvoiceId = id,
            WaivedAmount = request.WaivedAmount,
            Reason = request.Reason
        };

        var result = await _invoiceService.WaiveInvoiceAsync(id, command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelInvoiceRequestDto request)
    {
        var command = new CancelInvoiceCommand
        {
            InvoiceId = id,
            Reason = request.Reason
        };

        var result = await _invoiceService.CancelInvoiceAsync(id, command);
        return Ok(result);
    }

    /// <summary>
    /// Record payment for a specific invoice
    /// </summary>
    [HttpPost("{id:guid}/payments")]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordInvoicePaymentRequestDto dto)
    {
        try
        {
            // Map RequestDto to Command with InvoiceId from route
            var command = new RecordInvoicePaymentCommand
            {
                InvoiceId = id,
                Amount = dto.Amount,
                Method = dto.Method,
                PaidAt = dto.PaidAt,
                TransferFees = dto.TransferFees,
                ExternalReferenceCode = dto.ExternalReferenceCode,
                MethodDetailsJson = dto.MethodDetailsJson
            };

            var result = await _invoiceService.RecordPaymentAsync(command);
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
                title: "Payment recording error",
                detail: ex.Message
            );
        }
    }
}
