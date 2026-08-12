namespace SchoolManagement.Application.Core.Interfaces.Services;

public interface IPaymentService
{
    Task<List<PaymentResponseDto>> GetAllAsync();
    Task<PaymentResponseDto> GetByIdAsync(Guid id);
    Task<PaymentResponseDto> CreateAsync(RegistrationPaymentCommand command);
    Task<PaymentResponseDto> SettleChargeAsync(ChargeSettlementPaymentCommand command);
    Task<PaymentResponseDto> UpdateAsync(Guid id, UpdatePaymentCommand command);
    Task DeleteAsync(Guid id);
}
