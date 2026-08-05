using MediatR;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.DomainEvents;

namespace SchoolManagement.Application.Common.EventHandlers.WhatsApp;

/// <summary>
/// Sends WhatsApp notification when invoice is created/issued
/// </summary>
public class InvoiceCreatedWhatsAppHandler : INotificationHandler<InvoiceCreatedDomainEvent>
{
    private readonly IWhatsAppService _whatsAppService;
    private readonly IStudentQueryService _studentQueryService;
    private readonly IInvoiceQueryService _invoiceQueryService;

    public InvoiceCreatedWhatsAppHandler(
        IWhatsAppService whatsAppService,
        IStudentQueryService studentQueryService,
        IInvoiceQueryService invoiceQueryService)
    {
        _whatsAppService = whatsAppService;
        _studentQueryService = studentQueryService;
        _invoiceQueryService = invoiceQueryService;
    }

    public async Task Handle(InvoiceCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Fetch fresh invoice data with all details including TotalAmount (calculated from charge)
            var invoice = await _invoiceQueryService.GetByIdAsync(notification.InvoiceId);
            if (invoice == null) return;

            var student = await _studentQueryService.GetByIdAsync(invoice.Enrollment.StudentId);
            if (student == null || string.IsNullOrEmpty(student.Phone)) return;

            var message = $"Hello {student.FirstName},\n\n" +
                         $"Your invoice has been issued.\n\n" +
                         $"💰 Amount: {invoice.TotalAmount:N2} MAD\n" +
                         $"📅 Due Date: {notification.DueDate:yyyy-MM-dd}\n" +
                         $"📋 Period: {invoice.PeriodStart:yyyy-MM-dd} to {invoice.PeriodEnd:yyyy-MM-dd}\n\n" +
                         $"Please proceed with payment before the due date.\n\n" +
                         $"Thank you! 🙏";

            await _whatsAppService.QueueMessageAsync(
                phoneNumber: student.Phone,
                message: message,
                messageType: WhatsAppMessageType.InvoiceIssued,
                entityType: "Invoice",
                entityId: notification.InvoiceId
            );

            Console.WriteLine($"✅ Invoice issued WhatsApp queued for: {student.Phone}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Failed to queue invoice issued WhatsApp: {ex.Message}");
        }
    }
}
