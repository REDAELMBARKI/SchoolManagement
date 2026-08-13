using MediatR;
using Microsoft.Extensions.Logging;
using SchoolManagement.Domain.Common.Events;
using SchoolManagement.Application.Common.Interfaces.Services;

namespace SchoolManagement.Application.Common.EventHandlers;

/// <summary>
/// Handles welcome email requested events - SYNCHRONOUS for now (nice-to-have)
/// TODO: Can move to Hangfire background job later if needed
/// </summary>
public class WelcomeEmailRequestedEventHandler : INotificationHandler<WelcomeEmailRequestedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<WelcomeEmailRequestedEventHandler> _logger;

    public WelcomeEmailRequestedEventHandler(
        IEmailService emailService,
        ILogger<WelcomeEmailRequestedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(WelcomeEmailRequestedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Sending welcome email to {Email} (nice-to-have - synchronous for now)",
                notification.Email);

            // SYNCHRONOUS for now - simple and works
            // TODO: Move to Hangfire if you want background processing
            await _emailService.SendWelcomeEmailAsync(
                toEmail: notification.Email,
                userName: notification.UserName);

            _logger.LogInformation("Welcome email sent successfully to {Email}", notification.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", notification.Email);
        }
    }
}
