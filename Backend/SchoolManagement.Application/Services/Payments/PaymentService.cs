using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces.Queries;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IPaymentQueryService _query;
    private readonly ICurrentUserContext _currentUserContext;

    public PaymentService(IPaymentRepository repository , IPaymentQueryService paymentQueryService, ICurrentUserContext currentUserContext)
    {
        _repository = repository;
        _query = paymentQueryService;
        _currentUserContext = currentUserContext;
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

    public async Task<PaymentResponseDto> CreateAsync(PaymentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var payment = PaymentMapper.ToDomain(command);
        var createdPayment = await _repository.AddAsync(payment);
        return PaymentMapper.ToResponse(createdPayment);
    }

    public async Task<PaymentResponseDto> UpdateAsync(Guid id, UpdatePaymentCommand command)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");
        command.BranchId = branchId;

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"No payment found with id {id}");
        }

        existing.UpdateEnrollmentId(command.EnrollmentId);
        existing.UpdateAmount(command.Amount);
        existing.UpdateTransferFees(command.TransferFees);
        existing.UpdateMethod(command.Method);
        existing.UpdatePaidAt(command.PaidAt);
        existing.UpdateStatus(command.Status);
        existing.UpdateBranchId(command.BranchId);
        existing.UpdateReceivedByStaffId(command.ReceivedByStaffId);
        existing.UpdateExternalReferenceCode(command.ExternalReferenceCode);
        existing.UpdateMethodDetailsJson(command.MethodDetailsJson ?? "{}");

        var updated = await _repository.UpdateAsync(existing);
        return PaymentMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}