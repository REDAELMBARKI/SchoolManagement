using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SchoolManagement.Api.Settings;
using SchoolManagement.Application.Common.Interfaces.Services;

namespace SchoolManagement.Api.Services;

public class EmailService : IEmailService
{
    private readonly EmailTemplateService _templateService;
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        EmailTemplateService templateService,
        IOptions<SmtpSettings> smtpSettings,
        ILogger<EmailService> logger)
    {
        _templateService = templateService;
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            _logger.LogInformation($"connnectin smtp, source {_smtpSettings.Username}");


            // Connect to SMTP server
            await client.ConnectAsync(
                _smtpSettings.Host,
                _smtpSettings.Port,
                _smtpSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None
            );


            _logger.LogInformation($"smtp connected, {toEmail}, {subject}");


            // Authenticate if credentials provided
            if (!string.IsNullOrEmpty(_smtpSettings.Username) && !string.IsNullOrEmpty(_smtpSettings.Password))
            {
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            }

            // Send email
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {ToEmail} with subject: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            throw;
        }
    }

    public async Task SendPasswordResetEmailAsync(
        string toEmail,
        string userName,
        string resetUrl,
        string? ipAddress = null,
        string? userAgent = null)
    {
        try
        {
            var html = await _templateService.GeneratePasswordResetEmailAsync(
                userName: userName,
                resetUrl: resetUrl,
                ipAddress: ipAddress,
                userAgent: userAgent
            );

            await SendEmailAsync(toEmail, "Reset Your Password - School Management", html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", toEmail);
            throw;
        }
    }

    public async Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmUrl)
    {
        try
        {
            var html = await _templateService.GenerateEmailConfirmationAsync(
                userName: userName,
                confirmUrl: confirmUrl
            );

            await SendEmailAsync(toEmail, "Confirm Your Email - School Management", html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email confirmation to {Email}", toEmail);
            throw;
        }
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string userName)
    {
        try
        {
            var html = await _templateService.GenerateWelcomeEmailAsync(userName);

            await SendEmailAsync(toEmail, "Welcome to School Management! 🎉", html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
            throw;
        }
    }

    public async Task SendAccountLockedEmailAsync(
        string toEmail,
        string userName,
        string ipAddress,
        int failedAttempts)
    {
        try
        {
            var html = await _templateService.GenerateAccountLockedEmailAsync(
                userName: userName,
                ipAddress: ipAddress,
                failedAttempts: failedAttempts
            );

            await SendEmailAsync(toEmail, "Security Alert: Account Locked - School Management", html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send account locked email to {Email}", toEmail);
            throw;
        }
    }
}
