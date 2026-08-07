using SchoolManagement.Application.Core.Dtos.Commands;
using SchoolManagement.Application.Core.Dtos.Responses;

namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IPayrollPaymentService
{
    Task<List<PayrollPaymentResponseDto>> GetAllAsync();
    Task<PayrollPaymentResponseDto> GetByIdAsync(Guid id);
    Task<List<PayrollPaymentResponseDto>> GetByEmployeeIdAsync(Guid employeeId);
    Task<List<PayrollPaymentResponseDto>> GetByPeriodAsync(int year, int month);
    Task<PayrollPaymentResponseDto> CreateAsync(PayrollPaymentCommand command);
    Task<PayrollPaymentResponseDto> MarkAsPaidAsync(Guid id, MarkPayrollPaidCommand command);
    Task DeleteAsync(Guid id);
}
