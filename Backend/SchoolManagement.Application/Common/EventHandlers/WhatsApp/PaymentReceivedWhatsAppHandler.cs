using MediatR;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.DomainEvents;

namespace SchoolManagement.Application.Common.EventHandlers.WhatsApp;

/// <summary>
/// Sends payment receipt via WhatsApp when payment is received
/// </summary>
public class PaymentReceivedWhatsAppHandler : INotificationHandler<PaymentReceivedDomainEvent>
{
    private readonly IWhatsAppService _whatsAppService;
    private readonly IStudentQueryService _studentQueryService;
    private readonly IEnrollmentQueryService _enrollmentQueryService;
    private readonly IPaymentQueryService _paymentQueryService;

    public PaymentReceivedWhatsAppHandler(
        IWhatsAppService whatsAppService,
        IStudentQueryService studentQueryService,
        IEnrollmentQueryService enrollmentQueryService,
        IPaymentQueryService paymentQueryService)
    {
        _whatsAppService = whatsAppService;
        _studentQueryService = studentQueryService;
        _enrollmentQueryService = enrollmentQueryService;
        _paymentQueryService = paymentQueryService;
    }

    public async Task Handle(PaymentReceivedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentQueryService.GetByIdAsync(notification.EnrollmentId);
            if (enrollment == null) return;

            var student = await _studentQueryService.GetByIdAsync(enrollment.StudentId);
            if (student == null || string.IsNullOrEmpty(student.Phone)) return;

            var payment = await _paymentQueryService.GetByIdAsync(notification.PaymentId);
            if (payment == null) return;

            var message = $"Payment Received ✅\n\n" +
                         $"Hello {student.FirstName},\n\n" +
                         $"Your payment has been successfully processed.\n\n" +
                         $"💰 Amount: {notification.Amount:N2} MAD\n" +
                         $"📅 Date: {notification.PaidAt:yyyy-MM-dd HH:mm}\n" +
                         $"💳 Method: {payment.Method}\n" +
                         $"📋 Reference: {payment.Id.ToString().Substring(0, 8)}\n\n" +
                         $"Thank you for your payment! 🙏";

            await _whatsAppService.QueueMessageAsync(
                phoneNumber: student.Phone,
                message: message,
                messageType: WhatsAppMessageType.PaymentReceipt,
                entityType: "Payment",
                entityId: notification.PaymentId
            );

            Console.WriteLine($"✅ Payment receipt WhatsApp queued for: {student.Phone}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Failed to queue payment receipt WhatsApp: {ex.Message}");
        }
    }
}
