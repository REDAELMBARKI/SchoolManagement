using MediatR;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.DomainEvents;

namespace SchoolManagement.Application.Common.EventHandlers.WhatsApp;

/// <summary>
/// Sends welcome WhatsApp message when student enrolls
/// </summary>
public class EnrollmentCreatedWhatsAppHandler : INotificationHandler<EnrollmentCreatedDomainEvent>
{
    private readonly IWhatsAppService _whatsAppService;
    private readonly IStudentQueryService _studentQueryService;
    private readonly IEnrollmentQueryService _enrollmentQueryService;

    public EnrollmentCreatedWhatsAppHandler(
        IWhatsAppService whatsAppService,
        IStudentQueryService studentQueryService,
        IEnrollmentQueryService enrollmentQueryService)
    {
        _whatsAppService = whatsAppService;
        _studentQueryService = studentQueryService;
        _enrollmentQueryService = enrollmentQueryService;
    }

    public async Task Handle(EnrollmentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentQueryService.GetByIdAsync(notification.StudentId);
            if (student == null || string.IsNullOrEmpty(student.Phone)) return;

            var enrollment = await _enrollmentQueryService.GetByIdAsync(notification.EnrollmentId);
            if (enrollment == null) return;

            var message = $"Welcome {student.FirstName}! 🎉\n\n" +
                         $"You have been successfully enrolled in {enrollment.Subject?.Name ?? "your course"}.\n" +
                         $"Enrollment Date: {notification.EnrolledAt:yyyy-MM-dd}\n\n" +
                         $"We're excited to have you with us!\n\n" +
                         $"For any questions, feel free to contact us.";

            await _whatsAppService.QueueMessageAsync(
                phoneNumber: student.Phone,
                message: message,
                messageType: WhatsAppMessageType.EnrollmentWelcome,
                entityType: "Enrollment",
                entityId: notification.EnrollmentId
            );

            Console.WriteLine($"✅ Enrollment welcome WhatsApp queued for: {student.Phone}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Failed to queue enrollment welcome WhatsApp: {ex.Message}");
        }
    }
}
