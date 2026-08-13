namespace SchoolManagement.Application.Common.Interfaces.Services;

/// <summary>
/// Email service for sending emails
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetUrl, string? ipAddress = null, string? userAgent = null);
    Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmUrl);
    Task SendWelcomeEmailAsync(string toEmail, string userName);
    Task SendAccountLockedEmailAsync(string toEmail, string userName, string ipAddress, int failedAttempts);
}
