using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;

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
    public async Task<IActionResult> Create([FromBody] InvoiceCommand command)
    {
        var result = await _invoiceService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceCommand command)
    {
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
    public async Task<IActionResult> Waive(Guid id, [FromBody] WaiveInvoiceCommand command)
    {
        command.InvoiceId = id;
        var result = await _invoiceService.WaiveInvoiceAsync(id, command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelInvoiceCommand command)
    {
        command.InvoiceId = id;
        var result = await _invoiceService.CancelInvoiceAsync(id, command);
        return Ok(result);
    }
}
