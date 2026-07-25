using MediatR;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Application.Results;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Application.Services.Registrations;

public class StudentRegistrationService
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IStudentService _studentService;
    private readonly IChargeService _chargeService;
    private readonly IPaymentService _paymentService;
    private readonly ITransaction _transaction;
    private readonly IMediator _mediator;
    private readonly IPlanQueryService _planQueryService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IChargeRepository _chargeRepository;

    public StudentRegistrationService(
            IStudentService studentService,
            IEnrollmentService enrollmentService,
            IChargeService chargeService,
            IPaymentService paymentService,
            ITransaction transaction,
            IMediator mediator,
            IPlanQueryService planQueryService,
            ICurrentUserContext currentUserContext,
            IChargeRepository chargeRepository)
    {
        _studentService = studentService;
        _enrollmentService = enrollmentService;
        _chargeService = chargeService;
        _paymentService = paymentService;
        _transaction = transaction;
        _mediator = mediator;
        _planQueryService = planQueryService;
        _currentUserContext = currentUserContext;
        _chargeRepository = chargeRepository;
    }

    private async Task<EvaluatePaymentPlanResult> EvaluatePaymentPlanAsync(Guid planId, decimal amountPaid)
    {
        var result = new EvaluatePaymentPlanResult();
        var plan = await _planQueryService.GetByIdAsync(planId);
        if (plan == null)
            throw new NotFoundException($"Plan with id {planId} not found.");

        bool isFullyPaid = amountPaid >= plan.Amount;
        if (isFullyPaid)
        {
            result.IsFullyPaid = true;
        }
        else
        {
            result.RemainingAmountDueDays = plan.RemainingAmountDueDays;
            result.Amount = plan.Amount;
            result.PaidAmount = amountPaid;
        }

        return result;
    }

    public async Task<StudentRegistrationResponseDto> RegisterStudentAsync(StudentRegistrationRequestDto registrationRequestDto)
    {
        try
        {
            ChargeResponseDto? chargeResponse = null;

            await _transaction.BeginTransactionAsync();

            var branchId = _currentUserContext.BranchId;
            if (branchId == Guid.Empty)
                throw new DomainException("Branch context is missing.");

            var studentCommand = new StudentCommand
            {
                FirstName = registrationRequestDto.StudentRegReq.FirstName,
                LastName = registrationRequestDto.StudentRegReq.LastName,
                Email = registrationRequestDto.StudentRegReq.Email,
                Phone = registrationRequestDto.StudentRegReq.Phone,
                DateOfBirth = registrationRequestDto.StudentRegReq.DateOfBirth,
                GenderId = registrationRequestDto.StudentRegReq.GenderId,
                LevelId = registrationRequestDto.StudentRegReq.LevelId,
                IntakeId = registrationRequestDto.StudentRegReq.IntakeId,
                IsDirectRegistration = registrationRequestDto.StudentRegReq.IsDirectRegistration,
                BranchId = branchId
            };

            var studentResponse = await _studentService.CreateAsync(studentCommand);

            var enrollmentCommand = new EnrollmentCommand
            {
                StudentId = studentResponse.Id,
                LevelId = registrationRequestDto.EnrollmentRegReq.LevelId,
                SubjectId = registrationRequestDto.EnrollmentRegReq.SubjectId,
                PlanId = registrationRequestDto.EnrollmentRegReq.PlanId,
                Notes = registrationRequestDto.EnrollmentRegReq.Notes,
                BranchId = branchId,
                PreferedScheduleId = registrationRequestDto.EnrollmentRegReq.PreferedScheduleId,
                GroupId = registrationRequestDto.EnrollmentRegReq.GroupId ?? Guid.Empty
            };

            var enrollmentResponse = await _enrollmentService.CreateAsync(enrollmentCommand);
            var evaluatePaymentPlan = await EvaluatePaymentPlanAsync(enrollmentResponse.PlanId, registrationRequestDto.PaymentRegReq.Amount);

            if (!evaluatePaymentPlan.IsFullyPaid)
            {
                var chargeCommand = new ChargeCommand
                {
                    Amount = evaluatePaymentPlan.Amount,
                    AmountPaid = evaluatePaymentPlan.PaidAmount,
                    DueDate = DateTime.UtcNow.AddDays(evaluatePaymentPlan.RemainingAmountDueDays),
                    StudentId = studentResponse.Id,
                    SourceId = enrollmentResponse.Id,
                    ChargeType = ChargeType.Enrollment,
                    IssuedDate = DateTime.UtcNow,
                    BranchId = branchId
                };

                if (evaluatePaymentPlan.PaidAmount > 0)
                {
                    chargeCommand.Status = ChargeStatus.PartiallyPaid;
                }
                else
                {
                    chargeCommand.Status = ChargeStatus.Unpaid;
                }

                chargeResponse = await _chargeService.CreateAsync(chargeCommand);
            }

            PaymentResponseDto? paymentResponse = null;
            var paymentAmount = registrationRequestDto.PaymentRegReq?.Amount ?? 0;
            if (paymentAmount > 0)
            {
                var paymentCommand = new PaymentCommand
                {
                    EnrollmentId = enrollmentResponse.Id,
                    Amount = paymentAmount,
                    TransferFees = registrationRequestDto.PaymentRegReq!.TransferFees,
                    Method = registrationRequestDto.PaymentRegReq.Method,
                    ExternalReferenceCode = registrationRequestDto.PaymentRegReq.ExternalReferenceCode,
                    MethodDetailsJson = registrationRequestDto.PaymentRegReq.MethodDetailsJson ?? "{}",
                    BranchId = branchId,
                    ChargeId = chargeResponse?.Id ?? null,
                    ReceivedByStaffId = _currentUserContext.NameIdentifier,
                    CurrencyCode = registrationRequestDto.PaymentRegReq.CurrencyCode ?? "USD"
                };

                paymentResponse = await _paymentService.CreateAsync(paymentCommand);

                if (chargeResponse != null && chargeResponse.Id != Guid.Empty)
                {
                    var chargeEntity = await _chargeRepository.GetByIdAsync(chargeResponse.Id);
                    if (chargeEntity != null)
                    {
                        chargeEntity.AddPayment(paymentAmount);
                        await _chargeRepository.UpdateAsync(chargeEntity);
                        chargeResponse.AmountPaid = chargeEntity.AmountPaid;
                        chargeResponse.Status = chargeEntity.Status;
                    }
                }
            }

            await _mediator.Publish(new NewStudentAssignedDomainEvent(studentResponse.Id, enrollmentResponse.Id));
            await _transaction.CommitTransactionAsync();

            return new StudentRegistrationResponseDto
            {
                StudentRegRes = studentResponse,
                EnrollmentRegRes = enrollmentResponse,
                ChargeRegRes = chargeResponse,
                PaymentRegRes = paymentResponse
            };
        }
        catch (Exception)
        {
            await _transaction.RollbackTransactionAsync();
            throw;
        }
    }
}
