using SchoolManagement.Application.Common.Interfaces.Services;

namespace SchoolManagement.Tests.IntegrationTests;

public class FakeEmailService : IEmailService
{
    public Task SendAccountLockedEmailAsync(string toEmail, string userName, string ipAddress, int failedAttempts)
    {
        // No-op for tests
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        // No-op for tests
        return Task.CompletedTask;
    }

    public Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmUrl)
    {
        // No-op for tests
        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetUrl, string? ipAddress = null, string? userAgent = null)
    {
        // No-op for tests
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string toEmail, string userName)
    {
        // No-op for tests
        return Task.CompletedTask;
    }
}
