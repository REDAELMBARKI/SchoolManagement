using MediatR;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Results;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;

namespace SchoolManagement.Application.Services.Registrations;

public class StudentRegistrationService
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IStudentService _studentService;
    private readonly IInvoiceService _invoiceService;
    private readonly IPaymentService _paymentService;
    private readonly ITransaction _transaction;
    private readonly IMediator _mediator;
    private readonly IPlanQueryService _planQueryService;
    private readonly ICurrentUserContext _currentUserContext;

    public StudentRegistrationService(
        IStudentService studentService,
        IEnrollmentService enrollmentService,
        IInvoiceService invoiceService,
        IPaymentService paymentService,
        ITransaction transaction,
        IMediator mediator,
        IPlanQueryService planQueryService,
        ICurrentUserContext currentUserContext)
    {
        _studentService = studentService;
        _enrollmentService = enrollmentService;
        _invoiceService = invoiceService;
        _paymentService = paymentService;
        _transaction = transaction;
        _mediator = mediator;
        _planQueryService = planQueryService;
        _currentUserContext = currentUserContext;
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
            result.CreditBalance = amountPaid - plan.Amount;
        }
        else
        {
            result.RemainingAmountDueDays = plan.RemainingAmountDueDays;
            result.TotalAmount = plan.Amount;
            result.RemainingAmount = plan.Amount - amountPaid;
            result.PaidAmount = amountPaid;
        }

        return result;
    }

    public async Task<StudentRegistrationResponseDto> RegisterStudentAsync(StudentRegistrationRequestDto registrationRequestDto)
    {
        try
        {
            InvoiceResponseDto? invoiceResponse = null;
            EnrollmentResponseDto enrollmentResponse;

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
                Notes = registrationRequestDto.EnrollmentRegReq.Notes,
                BranchId = branchId,
                PreferedScheduleId = registrationRequestDto.EnrollmentRegReq.PreferedScheduleId,
                GroupId = registrationRequestDto.EnrollmentRegReq.GroupId ?? Guid.Empty
            };

            enrollmentResponse = await _enrollmentService.CreateAsync(enrollmentCommand);

            var paymentAmount = registrationRequestDto.PaymentRegReq?.AmountPaid ?? 0;
            var evaluatePaymentPlan = await EvaluatePaymentPlanAsync(
                registrationRequestDto.EnrollmentRegReq.PlanId,
                paymentAmount);

            var now = DateTime.UtcNow;
            var periodStart = registrationRequestDto.PeriodStart ?? now;
            var periodEnd = registrationRequestDto.PeriodEnd ?? now.AddMonths(1);

            // Create an invoice with Charge when payment is not fully paid upfront
            if (!evaluatePaymentPlan.IsFullyPaid)
            {
                var dueDate = registrationRequestDto.InvoiceDueDate
                    ?? now.AddDays(evaluatePaymentPlan.RemainingAmountDueDays);

                var invoiceCommand = new InvoiceCommand
                {
                    EnrollmentId = enrollmentResponse.Id,
                    PeriodStart = periodStart,
                    PeriodEnd = periodEnd,
                    DueDate = dueDate,
                    BranchId = branchId,
                    Charges = new List<ChargeCommand>
                    {
                        new ChargeCommand
                        {
                            Amount = evaluatePaymentPlan.TotalAmount
                        }
                    }
                };

                invoiceResponse = await _invoiceService.CreateAsync(invoiceCommand);
            }

            PaymentResponseDto? paymentResponse = null;
            if (paymentAmount > 0)
            {
                var paymentCommand = new RegistrationPaymentCommand
                {
                    EnrollmentId = enrollmentResponse.Id,
                    Amount = paymentAmount,
                    TransferFees = registrationRequestDto.PaymentRegReq!.TransferFees,
                    Method = registrationRequestDto.PaymentRegReq.Method,
                    ExternalReferenceCode = registrationRequestDto.PaymentRegReq.ExternalReferenceCode,
                    MethodDetailsJson = registrationRequestDto.PaymentRegReq.MethodDetailsJson ?? "{}",
                    BranchId = branchId,
                    InvoiceId = invoiceResponse?.Id,
                    ReceivedByStaffId = _currentUserContext.NameIdentifier,
                    CurrencyCode = registrationRequestDto.PaymentRegReq.CurrencyCode ?? "USD"
                };

                paymentResponse = await _paymentService.CreateAsync(paymentCommand);
            }

            if (evaluatePaymentPlan.CreditBalance > 0)
            {
                enrollmentResponse = await _enrollmentService.AddCreditAsync(
                    enrollmentResponse.Id,
                    evaluatePaymentPlan.CreditBalance);
            }

            await _mediator.Publish(new NewStudentAssignedDomainEvent(studentResponse.Id, enrollmentResponse.Id));
            await _transaction.CommitTransactionAsync();

            return new StudentRegistrationResponseDto
            {
                StudentRegRes = studentResponse,
                EnrollmentRegRes = enrollmentResponse,
                InvoiceRegRes = invoiceResponse,
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
