using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Domain.Core.Entities;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IPayrollPaymentQueryService : IEntityQuery<PayrollPayment>
{
    Task<List<PayrollPaymentResponseDto>> GetAllResponsesAsync();
    Task<PayrollPaymentResponseDto?> GetResponseByIdAsync(Guid id);
    Task<List<PayrollPayment>> GetByEmployeeIdAsync(Guid employeeId);
    Task<List<PayrollPayment>> GetByPeriodAsync(int year, int month);
}
