using MediatR;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.DomainEvents;

namespace SchoolManagement.Application.Common.EventHandlers.WhatsApp;

/// <summary>
/// Sends overdue reminder via WhatsApp when invoice becomes overdue
/// </summary>
public class InvoiceOverdueWhatsAppHandler : INotificationHandler<InvoiceOverdueDomainEvent>
{
    private readonly IWhatsAppService _whatsAppService;
    private readonly IStudentQueryService _studentQueryService;
    private readonly IEnrollmentQueryService _enrollmentQueryService;
    private readonly IInvoiceQueryService _invoiceQueryService;

    public InvoiceOverdueWhatsAppHandler(
        IWhatsAppService whatsAppService,
        IStudentQueryService studentQueryService,
        IEnrollmentQueryService enrollmentQueryService,
        IInvoiceQueryService invoiceQueryService)
    {
        _whatsAppService = whatsAppService;
        _studentQueryService = studentQueryService;
        _enrollmentQueryService = enrollmentQueryService;
        _invoiceQueryService = invoiceQueryService;
    }

    public async Task Handle(InvoiceOverdueDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await _invoiceQueryService.GetByIdAsync(notification.InvoiceId);
            if (invoice == null) return;

            var enrollment = await _enrollmentQueryService.GetByIdAsync(notification.EnrollmentId);
            if (enrollment == null) return;

            var student = await _studentQueryService.GetByIdAsync(enrollment.StudentId);
            if (student == null || string.IsNullOrEmpty(student.Phone)) return;

            var message = $"⚠️ Payment Reminder\n\n" +
                         $"Hello {student.FirstName},\n\n" +
                         $"This is a friendly reminder that your invoice is now overdue.\n\n" +
                         $"💰 Amount Due: {notification.AmountDue:N2} MAD\n" +
                         $"📅 Due Date: {invoice.DueDate:yyyy-MM-dd}\n" +
                         $"📆 Overdue Since: {notification.OverdueDate:yyyy-MM-dd}\n\n" +
                         $"Please make your payment as soon as possible to avoid any interruption in service.\n\n" +
                         $"Contact us if you need any assistance.";

            await _whatsAppService.QueueMessageAsync(
                phoneNumber: student.Phone,
                message: message,
                messageType: WhatsAppMessageType.OverdueInvoice,
                entityType: "Invoice",
                entityId: notification.InvoiceId
            );

            Console.WriteLine($"✅ Overdue invoice WhatsApp queued for: {student.Phone}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Failed to queue overdue invoice WhatsApp: {ex.Message}");
        }
    }
}
