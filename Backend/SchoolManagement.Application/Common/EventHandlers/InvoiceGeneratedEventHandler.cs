using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Domain.Common.Events;

namespace SchoolManagement.Application.Common.EventHandlers;

/// <summary>
/// Handles invoice generated events - PLACEHOLDER for Hangfire (not urgent)
/// TODO: Implement with Hangfire background job processing
/// </summary>
public class InvoiceGeneratedEventHandler : INotificationHandler<InvoiceGeneratedEvent>
{
    private readonly ILogger<InvoiceGeneratedEventHandler> _logger;

    public InvoiceGeneratedEventHandler(ILogger<InvoiceGeneratedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(InvoiceGeneratedEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Implement with Hangfire for background processing
        // For now, just log
        _logger.LogInformation(
            "Invoice {InvoiceNumber} generated for {Email} - PLACEHOLDER (implement with Hangfire later)",
            notification.InvoiceNumber,
            notification.Email);

        // Example Hangfire implementation (when you add it):
        // BackgroundJob.Enqueue<IEmailService>(x => 
        //     x.SendInvoiceEmailAsync(
        //         notification.Email, 
        //         notification.StudentName, 
        //         notification.InvoiceNumber,
        //         ...));

        return Task.CompletedTask;
    }
}
