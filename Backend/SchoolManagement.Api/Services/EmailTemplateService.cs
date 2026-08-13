using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Api.EmailTemplates;

namespace SchoolManagement.Api.Services;

public class EmailTemplateService
{
    private readonly IServiceProvider _serviceProvider;

    public EmailTemplateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Renders a Razor component to HTML string
    /// </summary>
    public async Task<string> RenderTemplateAsync<TComponent>(Dictionary<string, object?>? parameters = null) 
        where TComponent : IComponent
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var renderer = scope.ServiceProvider.GetRequiredService<HtmlRenderer>();

        var componentParameters = parameters != null 
            ? ParameterView.FromDictionary(parameters) 
            : ParameterView.Empty;

        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await renderer.RenderComponentAsync<TComponent>(componentParameters);
            return output.ToHtmlString();
        });

        return html;
    }

    /// <summary>
    /// Generates password reset email HTML
    /// </summary>
    public async Task<string> GeneratePasswordResetEmailAsync(
        string userName,
        string resetUrl,
        string? token = null,
        int expirationMinutes = 60,
        string? ipAddress = null,
        string? userAgent = null)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(EmailTemplates.PasswordResetEmail.UserName), userName },
            { nameof(EmailTemplates.PasswordResetEmail.ResetUrl), resetUrl },
            { nameof(EmailTemplates.PasswordResetEmail.Token), token },
            { nameof(EmailTemplates.PasswordResetEmail.ExpirationMinutes), expirationMinutes },
            { nameof(EmailTemplates.PasswordResetEmail.IpAddress), ipAddress ?? "Unknown" },
            { nameof(EmailTemplates.PasswordResetEmail.UserAgent), userAgent ?? "Unknown" },
            { nameof(EmailTemplates.PasswordResetEmail.RequestTime), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC") }
        };

        return await RenderTemplateAsync<PasswordResetEmail>(parameters);
    }

    /// <summary>
    /// Generates email confirmation email HTML
    /// </summary>
    public async Task<string> GenerateEmailConfirmationAsync(
        string userName,
        string confirmUrl,
        int expirationHours = 24)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(EmailTemplates.EmailConfirmation.UserName), userName },
            { nameof(EmailTemplates.EmailConfirmation.ConfirmUrl), confirmUrl },
            { nameof(EmailTemplates.EmailConfirmation.ExpirationHours), expirationHours }
        };

        return await RenderTemplateAsync<EmailConfirmation>(parameters);
    }

    /// <summary>
    /// Generates welcome email HTML
    /// </summary>
    public async Task<string> GenerateWelcomeEmailAsync(string userName)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(EmailTemplates.WelcomeEmail.UserName), userName }
        };

        return await RenderTemplateAsync<WelcomeEmail>(parameters);
    }

    /// <summary>
    /// Generates account locked email HTML
    /// </summary>
    public async Task<string> GenerateAccountLockedEmailAsync(
        string userName,
        string ipAddress,
        int failedAttempts = 5,
        int lockoutMinutes = 15)
    {
        var parameters = new Dictionary<string, object?>
        {
            { nameof(EmailTemplates.AccountLockedEmail.UserName), userName },
            { nameof(EmailTemplates.AccountLockedEmail.IpAddress), ipAddress },
            { nameof(EmailTemplates.AccountLockedEmail.FailedAttempts), failedAttempts },
            { nameof(EmailTemplates.AccountLockedEmail.LockoutMinutes), lockoutMinutes },
            { nameof(EmailTemplates.AccountLockedEmail.LockoutTime), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC") }
        };

        return await RenderTemplateAsync<AccountLockedEmail>(parameters);
    }
}
