using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces.Queries;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Application.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IPaymentQueryService _query;

    public PaymentService(IPaymentRepository repository , IPaymentQueryService paymentQueryService)
    {
        _repository = repository;
        _query = paymentQueryService;
    }

    public async Task<List<PaymentResponseDto>> GetAllAsync()
    {
        var payments = await _query.GetAllAsync();
        return payments.Select(p => PaymentMapper.ToResponse(p)).ToList();
    }

    public async Task<PaymentResponseDto?> GetByIdAsync(Guid id)
    {
        var payment = await _repository.GetByIdAsync(id);
        if (payment == null) return null;
        return PaymentMapper.ToResponse(payment);
    }

    public async Task<PaymentResponseDto> CreateAsync(PaymentRequestDto dto)
    {
        var payment = PaymentMapper.ToDomain(dto);
        var createdPayment = await _repository.AddAsync(payment);
        return PaymentMapper.ToResponse(createdPayment);
    }

    public async Task<PaymentResponseDto> UpdateAsync(Guid id, UpdatePaymentRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"No payment found with id {id}");
        }

        existing.UpdateEnrollmentId(dto.EnrollmentId);
        existing.UpdateFeeAmount(dto.FeeAmount);
        existing.UpdatePeriodStart(dto.PeriodStart);
        existing.UpdatePeriodEnd(dto.PeriodEnd);
        existing.UpdateAmountPaid(dto.AmountPaid);
        existing.UpdatePaidAt(dto.PaidAt);
        existing.UpdateStatus(dto.Status);

        var updated = await _repository.UpdateAsync(existing);
        return PaymentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}