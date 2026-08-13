# Email Templates Setup Guide

## 📦 Step 1: Install Required Package

```bash
cd SchoolManagement.Infrastructure
dotnet add package Microsoft.AspNetCore.Components.Web
```

## 🔧 Step 2: Register Services in Program.cs

Add this to your `Program.cs` in the API project:

```csharp
// Email Templates & Service
builder.Services.AddRazorComponents();
builder.Services.AddScoped<EmailTemplateService>();
builder.Services.AddScoped<IEmailService, EmailService>();
```

## 📝 Step 3: Update ForgotPassword Endpoint

Update `AccountController.cs`:

```csharp
private readonly IEmailService _emailService; // Add this field

public AccountController(
    // ... existing parameters
    IEmailService emailService) // Add this parameter
{
    // ... existing assignments
    _emailService = emailService;
}

[HttpPost("forgot-password")]
[AllowAnonymous]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
{
    try
    {
        var applicationUserId = await _authService.GetUserIdByEmailAsync(request.Email);
        
        if (applicationUserId != null)
        {
            var token = await _authService.GeneratePasswordResetTokenAsync(applicationUserId);
            
            // Build reset URL
            var resetUrl = $"{Request.Scheme}://{Request.Host}/reset-password?token={token}&userId={applicationUserId}";
            
            // Get user details
            var user = await _authService.GetApplicationUserAsync(applicationUserId);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            
            // Send email with beautiful template
            await _emailService.SendPasswordResetEmailAsync(
                toEmail: request.Email,
                userName: user.UserName ?? "User",
                resetUrl: resetUrl,
                ipAddress: ipAddress,
                userAgent: userAgent
            );
        }

        return Ok(new
        {
            message = "If an account exists with this email, a password reset link has been sent."
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "An error occurred. Please try again later." });
    }
}
```

## 📧 Step 4: Implement Actual Email Sending

Choose your email provider:

### Option A: SendGrid (Recommended)

```bash
dotnet add package SendGrid
```

Update `EmailService.cs`:

```csharp
private readonly ISendGridClient _sendGridClient;

public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
{
    var from = new EmailAddress("noreply@yourschool.com", "School Management");
    var to = new EmailAddress(toEmail);
    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlBody);
    
    var response = await _sendGridClient.SendEmailAsync(msg);
    
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Failed to send email: {response.StatusCode}");
    }
}
```

Add to `appsettings.json`:
```json
{
  "SendGrid": {
    "ApiKey": "YOUR_SENDGRID_API_KEY"
  }
}
```

### Option B: SMTP (Gmail, Office365, etc.)

```bash
dotnet add package MailKit
```

```csharp
public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
{
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("School Management", "noreply@yourschool.com"));
    message.To.Add(new MailboxAddress("", toEmail));
    message.Subject = subject;
    message.Body = new TextPart("html") { Text = htmlBody };

    using var client = new SmtpClient();
    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
    await client.AuthenticateAsync("your-email@gmail.com", "your-app-password");
    await client.SendAsync(message);
    await client.DisconnectAsync(true);
}
```

### Option C: AWS SES

```bash
dotnet add package AWSSDK.SimpleEmail
```

```csharp
private readonly IAmazonSimpleEmailService _sesClient;

public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
{
    var request = new SendEmailRequest
    {
        Source = "noreply@yourschool.com",
        Destination = new Destination { ToAddresses = new List<string> { toEmail } },
        Message = new Message
        {
            Subject = new Content(subject),
            Body = new Body { Html = new Content(htmlBody) }
        }
    };

    await _sesClient.SendEmailAsync(request);
}
```

## 🎨 Step 5: Customize Templates

### Update Brand Colors

Edit any `.razor` file and change the gradient colors:

```css
/* Current */
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

/* Your brand colors */
background: linear-gradient(135deg, #YOUR_COLOR1 0%, #YOUR_COLOR2 100%);
```

### Add Your Logo

In the header section of any template:

```html
<div class="header">
    <img src="https://yourschool.com/logo.png" alt="Logo" style="max-width: 150px; margin-bottom: 20px;">
    <h1>🔐 Password Reset Request</h1>
</div>
```

## 🧪 Step 6: Test Templates

Create a test endpoint:

```csharp
[HttpGet("test-reset-email")]
[AllowAnonymous]
public async Task<IActionResult> TestResetEmail()
{
    var html = await _emailTemplateService.GeneratePasswordResetEmailAsync(
        userName: "Test User",
        resetUrl: "https://yourschool.com/reset-password?token=test123",
        ipAddress: "192.168.1.1",
        userAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
    );
    
    return Content(html, "text/html");
}
```

Navigate to `/test-reset-email` in your browser to preview.

## ✅ Done!

Your email system is now ready. Templates will automatically render with beautiful, responsive HTML.

## 🌍 For Morocco Deployment

Update email content:

1. Add Arabic/French translations
2. Update time zones in templates
3. Add local support contact
4. Update privacy policy links
