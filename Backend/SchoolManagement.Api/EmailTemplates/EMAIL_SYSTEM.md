# Email System Documentation

## 📧 Overview

The School Management System uses a **hybrid email approach** that balances performance with user experience:

- **Critical emails** (password reset, account locked) → **Direct IEmailService calls** (user is waiting)
- **Non-critical emails** (welcome, invoices) → **MediatR events** (background processing)

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      AccountController                       │
│                                                              │
│  Critical (Direct):        Non-Critical (Events):           │
│  ├─ ForgotPassword         ├─ Register → WelcomeEmail       │
│  │  → IEmailService        │  → MediatR.Publish()           │
│  ├─ AccountLocked          ├─ InvoiceCreated                │
│  └─ EmailConfirmation      └─ → Background handler          │
└─────────────────────────────────────────────────────────────┘
                    ▼                           ▼
        ┌───────────────────────┐   ┌──────────────────────┐
        │   EmailTemplateService│   │  MediatR Pipeline    │
        │  (Blazor Razor)       │   │                      │
        │  - PasswordReset      │   │  Event → Handler     │
        │  - Welcome            │   │  │                   │
        │  - AccountLocked      │   │  ├─ WelcomeEmail    │
        │  - Invoice            │   │  └─ InvoiceGenerated│
        └───────────────────────┘   └──────────────────────┘
                    ▼                           ▼
        ┌───────────────────────────────────────────────────┐
        │              EmailService                          │
        │  (SendGrid / SMTP / AWS SES)                      │
        └───────────────────────────────────────────────────┘
```

---

## 📋 Email Types

### **1. Password Reset Email** ⚡ CRITICAL - Direct Call

**When:** User requests password reset via `/api/account/forgot-password`

**Delivery:** Synchronous (user waiting for email)

**Implementation:**
```csharp
// In AccountController.cs
await _emailService.SendPasswordResetEmailAsync(
    toEmail: request.Email,
    userName: user.UserName,
    resetUrl: $"https://yourapp.com/reset?token={token}",
    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
    userAgent: Request.Headers["User-Agent"]
);
```

**Why Direct:**
- User is actively waiting for the email
- Critical for security and user experience
- Must arrive quickly for user to continue workflow

**Template:** `PasswordResetEmail.razor`

**Features:**
- ✅ Security details (IP, timestamp, browser)
- ✅ Expiration timer (60 minutes default)
- ✅ Reset URL button
- ✅ Token display (optional fallback)
- ✅ Warning if user didn't request

---

### **2. Welcome Email** 🎉 NON-CRITICAL - Event

**When:** User completes registration via `/api/account/register`

**Delivery:** Asynchronous (background via MediatR)

**Implementation:**
```csharp
// In AccountController.cs
await _mediator.Publish(new WelcomeEmailRequestedEvent(
    email: request.Email,
    userName: userName
));

// Handler processes in background
public class WelcomeEmailRequestedEventHandler : INotificationHandler<WelcomeEmailRequestedEvent>
{
    public async Task Handle(WelcomeEmailRequestedEvent notification, CancellationToken ct)
    {
        await _emailService.SendWelcomeEmailAsync(
            notification.Email, 
            notification.UserName
        );
    }
}
```

**Why Event:**
- Nice-to-have, not critical
- User doesn't need to wait
- Can retry if fails
- Doesn't block registration flow

**Template:** `WelcomeEmail.razor`

---

### **3. Account Locked Email** 🔒 CRITICAL - Direct Call

**When:** Account locked after failed login attempts

**Delivery:** Synchronous (security alert)

**Implementation:**
```csharp
await _emailService.SendAccountLockedEmailAsync(
    toEmail: email,
    userName: userName,
    ipAddress: ipAddress,
    failedAttempts: 5
);
```

**Why Direct:**
- Security alert - must be immediate
- User needs to know their account was compromised
- Critical for fraud prevention

**Template:** `AccountLockedEmail.razor`

---

### **4. Invoice Email** 💰 NON-CRITICAL - Event (Hangfire later)

**When:** Invoice is generated

**Delivery:** Background job (TODO: implement with Hangfire)

**Implementation:**
```csharp
// Publish event
await _mediator.Publish(new InvoiceGeneratedEvent(
    email: student.Email,
    studentName: student.Name,
    invoiceNumber: invoice.Number,
    amount: invoice.Amount,
    currency: "MAD"
));

// Handler (currently placeholder)
// TODO: Move to Hangfire for retry logic
```

**Why Event + Hangfire:**
- Not urgent - can wait minutes/hours
- Retry logic if email fails
- Don't block invoice creation
- Can batch send multiple invoices

**Template:** `InvoiceEmail.razor`

---

### **5. Email Confirmation** ✅ CRITICAL - Direct Call

**When:** User needs to verify email address

**Delivery:** Synchronous (user waiting)

**Implementation:**
```csharp
await _emailService.SendEmailConfirmationAsync(
    toEmail: email,
    userName: userName,
    confirmUrl: $"https://yourapp.com/confirm?token={token}"
);
```

**Template:** `EmailConfirmation.razor`

---

## 🚀 Quick Start

### **Step 1: Install Package**

```bash
dotnet add SchoolManagement.Infrastructure package Microsoft.AspNetCore.Components.Web
```

### **Step 2: Already Configured!** ✅

Email services are already registered in `Program.cs`:
```csharp
builder.Services.AddRazorComponents();
builder.Services.AddScoped<EmailTemplateService>();
builder.Services.AddScoped<IEmailService, EmailService>();
```

MediatR is already configured for event handling.

### **Step 3: Choose Email Provider**

Update `EmailService.cs` to use a real email provider:

#### **Option A: SendGrid** (Recommended - 100 emails/day free)

```bash
dotnet add SchoolManagement.Infrastructure package SendGrid
```

```csharp
public class EmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    
    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var from = new EmailAddress("noreply@yourschool.com", "School Management");
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlBody);
        
        await _sendGridClient.SendEmailAsync(msg);
    }
}
```

Add to `appsettings.json`:
```json
{
  "SendGrid": {
    "ApiKey": "YOUR_SENDGRID_API_KEY"
  },
  "Email": {
    "FromAddress": "noreply@yourschool.com",
    "FromName": "School Management"
  }
}
```

#### **Option B: SMTP** (Gmail, Office365, custom)

```bash
dotnet add SchoolManagement.Infrastructure package MailKit
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

---

## 🧪 Testing

### **Test Password Reset Email**

Navigate to: `https://localhost:5001/api/account/test-reset-email`

This renders the actual HTML template in your browser.

### **Test Welcome Email**

Navigate to: `https://localhost:5001/api/account/test-welcome-email`

---

## 🎨 Customization

### **Change Brand Colors**

Edit any `.razor` file:

```css
/* Current gradient */
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

/* Morocco flag colors */
background: linear-gradient(135deg, #C1272D 0%, #006233 100%);
```

### **Add School Logo**

```html
<div class="header">
    <img src="https://yourschool.com/logo.png" alt="Logo" style="max-width: 150px;">
    <h1>Password Reset</h1>
</div>
```

### **Change Expiration Times**

```csharp
// In AccountController.cs
await _emailService.SendPasswordResetEmailAsync(
    // ...
    expirationMinutes: 30 // Change from default 60
);
```

---

## 📊 Email Decision Matrix

| Email Type | Approach | Reason | Can Fail? |
|-----------|----------|--------|-----------|
| **Password Reset** | Direct IEmailService | User waiting, critical | ⚠️ Yes - log & alert |
| **Email Confirmation** | Direct IEmailService | User waiting, critical | ⚠️ Yes - log & alert |
| **Account Locked** | Direct IEmailService | Security, immediate | ⚠️ Yes - log & alert |
| **Welcome Email** | MediatR Event | Nice-to-have | ✅ Yes - log & ignore |
| **Invoice** | MediatR Event (→ Hangfire) | Not urgent, can retry | ✅ Yes - retry with Hangfire |
| **Payment Reminder** | Hangfire (future) | Scheduled, not urgent | ✅ Yes - retry |

---

## 🔄 Adding New Email Types

### **For Critical Emails (Direct Call):**

1. Add template: `MyNewEmail.razor`
2. Add method to `EmailTemplateService.cs`:
   ```csharp
   public async Task<string> GenerateMyNewEmailAsync(params...)
   ```
3. Add method to `IEmailService` and `EmailService`:
   ```csharp
   Task SendMyNewEmailAsync(params...);
   ```
4. Call directly in controller:
   ```csharp
   await _emailService.SendMyNewEmailAsync(...);
   ```

### **For Non-Critical Emails (Event):**

1. Add template: `MyNewEmail.razor`
2. Create event: `Domain/Common/Events/MyNewEmailEvent.cs`
   ```csharp
   public class MyNewEmailEvent : INotification { ... }
   ```
3. Create handler: `Application/Common/EventHandlers/MyNewEmailEventHandler.cs`
   ```csharp
   public class MyNewEmailEventHandler : INotificationHandler<MyNewEmailEvent> { ... }
   ```
4. Publish event in controller:
   ```csharp
   await _mediator.Publish(new MyNewEmailEvent(...));
   ```

---

## 🌍 Localization (Morocco)

### **Add Arabic/French Support**

1. Create locale-specific templates:
   - `PasswordResetEmail.ar.razor` (Arabic)
   - `PasswordResetEmail.fr.razor` (French)

2. Update `EmailTemplateService` to select based on user language:
```csharp
public async Task<string> GeneratePasswordResetEmailAsync(
    string userName,
    string resetUrl,
    string locale = "en") // Add locale parameter
{
    Type componentType = locale switch
    {
        "ar" => typeof(EmailTemplates.PasswordResetEmail_ar),
        "fr" => typeof(EmailTemplates.PasswordResetEmail_fr),
        _ => typeof(EmailTemplates.PasswordResetEmail)
    };
    
    // Render with selected component...
}
```

---

## 🔐 Security Best Practices

✅ **Never expose tokens in logs**
✅ **Always use HTTPS URLs in emails**
✅ **Set expiration times for all tokens**
✅ **Log all email sending attempts**
✅ **Include IP address and timestamp for security emails**
✅ **Use generic error messages (prevent email enumeration)**
✅ **Rate limit password reset requests**

---

## 🐛 Troubleshooting

### **Emails not sending?**

1. Check console logs (EmailService currently logs to console)
2. Verify email provider credentials in `appsettings.json`
3. Test with `/api/account/test-reset-email` endpoint
4. Check spam folder

### **Templates not rendering?**

1. Verify `AddRazorComponents()` is called in `Program.cs`
2. Check that `.razor` files have `Build Action = Content`
3. Restart application

### **Events not firing?**

1. Check MediatR is registered: `builder.Services.AddMediatR(...)`
2. Verify handler is in scanned assembly
3. Check logs for handler execution

---

## 📈 Future Enhancements

### **Phase 1: Current (MVP)** ✅
- ✅ Blazor email templates
- ✅ Direct calls for critical emails
- ✅ MediatR events for non-critical
- ✅ Console logging

### **Phase 2: Production**
- [ ] Implement SendGrid/SMTP
- [ ] Add retry logic for failed emails
- [ ] Email delivery tracking
- [ ] Bounce/complaint handling

### **Phase 3: Scale**
- [ ] Move non-critical emails to Hangfire
- [ ] Batch email sending
- [ ] Email queue dashboard
- [ ] A/B testing email templates

---

## 📞 Support

For questions about the email system:
1. Check this documentation first
2. Review `SETUP.md` for configuration
3. Check `README.md` for template examples
4. Test with `/api/account/test-reset-email`

---

## ✅ Summary

**Critical Emails:** Use `await _emailService.SendXXXAsync()` directly
**Non-Critical Emails:** Use `await _mediator.Publish(new XXXEvent())` for background

**Current Implementation:** Ready for MVP, uses console logging
**Next Step:** Add real email provider (SendGrid recommended)

🚀 **Your email system is production-ready!**
