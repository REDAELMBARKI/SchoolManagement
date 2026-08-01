using MediatR;
using SchoolManagement.Domain.Core.DomainEvents;
using SchoolManagement.Domain.Core.Interfaces;

namespace SchoolManagement.Application.Core.EventsHandlers;

public class InvoiceOverdueNotificationHandler : INotificationHandler<InvoiceOverdueDomainEvent>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public InvoiceOverdueNotificationHandler(
        IStudentRepository studentRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task Handle(InvoiceOverdueDomainEvent notification, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(notification.EnrollmentId);
        if (enrollment == null) return;

        var student = await _studentRepository.GetByIdAsync(enrollment.StudentId);
        if (student == null) return;

        string phone;
        string? email;

        if (student.StudentResponsables.Any())
        {
            var responsible = student.StudentResponsables.First();
            phone = responsible.Phone;
            email = responsible.Email;
        }
        else
        {
            phone = student.Phone;
            email = student.Email?.Value;
        }

        await SendSmsNotification(phone, notification.AmountDue, notification.OverdueDate);
        if (!string.IsNullOrWhiteSpace(email))
        {
            await SendEmailNotification(email, notification.AmountDue, notification.OverdueDate);
        }
    }

    private Task SendSmsNotification(string phone, decimal amountDue, DateTime overdueDate)
    {
        // TODO: Integrate with SMS service (e.g., Twilio, MessageBird)
        // Placeholder implementation
        return Task.CompletedTask;
    }

    private Task SendEmailNotification(string email, decimal amountDue, DateTime overdueDate)
    {
        // TODO: Integrate with email service (e.g., SendGrid, Mailgun)
        // Placeholder implementation
        return Task.CompletedTask;
    }
}
