using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Responses;

namespace SchoolManagement.Application.Interfaces.Services;

public interface IInvoiceService
{
    Task<List<InvoiceResponseDto>> GetAllAsync();
    Task<InvoiceResponseDto?> GetByIdAsync(Guid id);
    Task<InvoiceResponseDto> CreateAsync(InvoiceCommand command);
    Task<InvoiceResponseDto> UpdateAsync(Guid id, UpdateInvoiceCommand command);
    Task DeleteAsync(Guid id);
    Task<InvoiceResponseDto> WaiveInvoiceAsync(Guid id, WaiveInvoiceCommand command);
    Task<InvoiceResponseDto> CancelInvoiceAsync(Guid id, CancelInvoiceCommand command);
    Task ProcessPastDueInvoicesAsync();
    Task GenerateDailyInvoicesAsync();
}
