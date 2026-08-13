# Email Templates

This folder contains Razor-based email templates for the School Management System.

## 📧 Available Templates

### 1. **PasswordResetEmail.razor**
Used when a user requests to reset their password.

**Parameters:**
- `UserName` - User's display name
- `ResetUrl` - Full URL to reset password page
- `Token` - Optional token to display (if not using URL)
- `ExpirationMinutes` - How long the reset link is valid (default: 60)
- `IpAddress` - IP address of the request
- `UserAgent` - Browser/device information
- `RequestTime` - When the request was made
- `SupportUrl` - Support contact link
- `PrivacyUrl` - Privacy policy link

**Example:**
```csharp
var html = await _emailTemplateService.GeneratePasswordResetEmailAsync(
    userName: "John Doe",
    resetUrl: "https://yourapp.com/reset-password?token=abc123",
    ipAddress: "192.168.1.1",
    userAgent: "Chrome/Safari"
);
```

---

### 2. **EmailConfirmation.razor**
Used when a new user registers and needs to confirm their email.

**Parameters:**
- `UserName` - User's display name
- `ConfirmUrl` - Full URL to confirm email
- `ExpirationHours` - How long the confirmation link is valid (default: 24)

**Example:**
```csharp
var html = await _emailTemplateService.GenerateEmailConfirmationAsync(
    userName: "Jane Smith",
    confirmUrl: "https://yourapp.com/confirm-email?token=xyz789"
);
```

---

### 3. **WelcomeEmail.razor**
Used to welcome new users after successful registration.

**Parameters:**
- `UserName` - User's display name

**Example:**
```csharp
var html = await _emailTemplateService.GenerateWelcomeEmailAsync(
    userName: "Ahmed Ali"
);
```

---

### 4. **AccountLockedEmail.razor**
Used when a user's account is locked due to too many failed login attempts.

**Parameters:**
- `UserName` - User's display name
- `IpAddress` - IP address of failed attempts
- `FailedAttempts` - Number of failed login attempts (default: 5)
- `LockoutMinutes` - How long the account will be locked (default: 15)
- `LockoutTime` - When the lockout occurred

**Example:**
```csharp
var html = await _emailTemplateService.GenerateAccountLockedEmailAsync(
    userName: "Mohammed Hassan",
    ipAddress: "192.168.1.100",
    failedAttempts: 5
);
```

---

## 🚀 Setup

### 1. Add Required NuGet Package
```bash
dotnet add package Microsoft.AspNetCore.Components.Web
```

### 2. Register Services in Program.cs
```csharp
// Add Razor component services for email templates
builder.Services.AddRazorComponents();
builder.Services.AddScoped<EmailTemplateService>();
```

### 3. Use in Your Code
```csharp
public class YourService
{
    private readonly EmailTemplateService _emailTemplateService;
    
    public YourService(EmailTemplateService emailTemplateService)
    {
        _emailTemplateService = emailTemplateService;
    }
    
    public async Task SendPasswordResetAsync(string email, string resetUrl)
    {
        var html = await _emailTemplateService.GeneratePasswordResetEmailAsync(
            userName: "User",
            resetUrl: resetUrl,
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()
        );
        
        // Send email using your email service
        await _emailService.SendAsync(email, "Reset Your Password", html);
    }
}
```

---

## 🎨 Customization

### Modify Template Styling
Edit the `<style>` section in each `.razor` file to match your brand colors and design.

### Add New Templates
1. Create a new `.razor` file in this folder
2. Add `@code { [Parameter] ... }` for parameters
3. Add a method in `EmailTemplateService.cs` to render it

**Example:**
```razor
@* InvoiceEmail.razor *@
<!DOCTYPE html>
<html>
<body>
    <h1>Invoice for @InvoiceName</h1>
    <p>Amount: @Amount</p>
</body>
</html>

@code {
    [Parameter] public string InvoiceName { get; set; } = "";
    [Parameter] public decimal Amount { get; set; }
}
```

Then add to `EmailTemplateService.cs`:
```csharp
public async Task<string> GenerateInvoiceEmailAsync(string invoiceName, decimal amount)
{
    var parameters = new Dictionary<string, object?>
    {
        { "InvoiceName", invoiceName },
        { "Amount", amount }
    };
    return await RenderTemplateAsync<EmailTemplates.InvoiceEmail>(parameters);
}
```

---

## 📱 Responsive Design

All templates are mobile-responsive with:
- Max-width: 600px (optimal for email clients)
- Readable fonts and spacing
- Tested on Gmail, Outlook, Apple Mail

---

## 🔐 Security Notes

- Never include sensitive data directly in URLs (use tokens)
- Always use HTTPS for reset/confirmation URLs
- Set appropriate expiration times
- Log all email sending attempts
- Track IP addresses for security audit

---

## 🌍 Localization

To add Arabic/French translations:

1. Create locale-specific templates (e.g., `PasswordResetEmail.ar.razor`)
2. Add locale parameter to `EmailTemplateService`
3. Select template based on user's preferred language

---

## 📊 Testing

Test templates by rendering them in a browser:

```csharp
[HttpGet("test-email")]
public async Task<IActionResult> TestEmail()
{
    var html = await _emailTemplateService.GeneratePasswordResetEmailAsync(
        userName: "Test User",
        resetUrl: "https://example.com/reset"
    );
    return Content(html, "text/html");
}
```

Navigate to `/test-email` to preview the rendered template.
