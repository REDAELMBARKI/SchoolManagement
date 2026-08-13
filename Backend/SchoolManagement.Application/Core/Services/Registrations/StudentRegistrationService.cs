using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Core.Interfaces.Services;

namespace SchoolManagement.Application.Core.Services;

/// <summary>
/// Pure orchestration service for student registration.
/// Delegates to existing services: StudentService, EnrollmentService, InvoiceService, PaymentService.
/// </summary>
public class StudentRegistrationService : IStudentRegistrationService
{
    private readonly IStudentService _studentService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IInvoiceService _invoiceService;
    private readonly IPaymentService _paymentService;
    private readonly IStudentResponsableService _responsableService;
    private readonly ICurrentUserContext _currentUserContext;

    public StudentRegistrationService(
        IStudentService studentService,
        IEnrollmentService enrollmentService,
        IInvoiceService invoiceService,
        IPaymentService paymentService,
        IStudentResponsableService responsableService,
        ICurrentUserContext currentUserContext)
    {
        _studentService = studentService;
        _enrollmentService = enrollmentService;
        _invoiceService = invoiceService;
        _paymentService = paymentService;
        _responsableService = responsableService;
        _currentUserContext = currentUserContext;
    }

    public async Task<StudentRegistrationResponseDto> RegisterStudentAsync(StudentRegistrationRequestDto request)
    {
        // 1. Create Student using StudentService
        var studentCommand = new StudentCommand
        {
            FirstName = request.StudentRegReq.FirstName,
            LastName = request.StudentRegReq.LastName,
            Email = request.StudentRegReq.Email,
            Phone = request.StudentRegReq.Phone,
            DateOfBirth = request.StudentRegReq.DateOfBirth,
            GenderId = request.StudentRegReq.GenderId,
            LevelId = request.StudentRegReq.LevelId,
            IntakeId = request.StudentRegReq.IntakeId,
            IsDirectRegistration = request.StudentRegReq.IsDirectRegistration,
            BranchId = _currentUserContext.BranchId
        };

        var createdStudent = await _studentService.CreateAsync(studentCommand);

        // 2. Create Parent/Guardian if provided and link to student using StudentResponsableService
        if (request.ResponsableRegReq != null)
        {
            await _responsableService.CreateAndLinkToStudentAsync(createdStudent.Id, request.ResponsableRegReq);
        }

        // 3. Create Enrollment using EnrollmentService
        var enrollmentCommand = new EnrollmentCommand
        {
            StudentId = createdStudent.Id,
            SubjectId = request.EnrollmentRegReq.SubjectId,
            PreferedGroupId = request.EnrollmentRegReq.PreferedGroupId ?? Guid.Empty,
            LevelId = request.EnrollmentRegReq.LevelId,
            PlanId = request.EnrollmentRegReq.PlanId,
            BranchId = _currentUserContext.BranchId,
            EnrolledAt = DateTime.UtcNow,
            Status = EnrollmentStatus.Active
        };

        var createdEnrollment = await _enrollmentService.CreateAsync(enrollmentCommand);

        // 4. Create Invoice using InvoiceService
        var invoiceCommand = new InvoiceCommand
        {
            EnrollmentId = createdEnrollment.Id,
            PeriodStart = request.PeriodStart ?? DateTime.UtcNow,
            PeriodEnd = request.PeriodEnd ?? DateTime.UtcNow.AddMonths(1),
            DueDate = request.InvoiceDueDate ?? DateTime.UtcNow.AddDays(7),
            BranchId = _currentUserContext.BranchId
        };

        var createdInvoice = await _invoiceService.CreateAsync(invoiceCommand);

        // 5. Create Payment using PaymentService
        var paymentCommand = new RegistrationPaymentCommand
        {
            EnrollmentId = createdEnrollment.Id,
            InvoiceId = createdInvoice.Id,
            Amount = request.PaymentRegReq.AmountPaid,
            TransferFees = null,
            Method = request.PaymentRegReq.Method,
            Status = PaymentStatus.Completed,
            PaidAt = DateTime.UtcNow,
            BranchId = _currentUserContext.BranchId ,
            ReceivedByStaffId = _currentUserContext.NameIdentifier,
            ExternalReferenceCode = null,
            MethodDetailsJson = "{}"
        };

        var createdPayment = await _paymentService.CreateAsync(paymentCommand);

        return new StudentRegistrationResponseDto
        {
            StudentRegRes = createdStudent,
            EnrollmentRegRes = createdEnrollment,
            InvoiceRegRes = createdInvoice,
            PaymentRegRes = createdPayment
        };
    }
}
