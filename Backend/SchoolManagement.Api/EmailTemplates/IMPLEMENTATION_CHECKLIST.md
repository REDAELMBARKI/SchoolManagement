# Email System Implementation Checklist ✅

Use this checklist to verify everything is working correctly.

---

## 📋 Setup Phase

### **1. Package Installation**
```bash
cd SchoolManagement.Infrastructure
dotnet add package Microsoft.AspNetCore.Components.Web
```

- [ ] Package installed successfully
- [ ] No build errors

### **2. Verify Service Registration**

Check `SchoolManagement.Api/Program.cs` contains:
```csharp
builder.Services.AddRazorComponents();
builder.Services.AddScoped<EmailTemplateService>();
builder.Services.AddScoped<IEmailService, EmailService>();
```

- [ ] All three services registered
- [ ] No compilation errors

### **3. Build Project**
```bash
dotnet build
```

- [ ] Build succeeds
- [ ] No warnings about email services

---

## 🧪 Testing Phase

### **4. Test Template Rendering**

Start the API:
```bash
dotnet run --project SchoolManagement.Api
```

Navigate to test endpoints:

#### **Password Reset Email**
URL: `https://localhost:5001/api/account/test-reset-email`

- [ ] Page loads without errors
- [ ] Email template renders in browser
- [ ] Gradient colors visible
- [ ] Reset button displayed
- [ ] Security info shown (IP, timestamp)

#### **Welcome Email**
URL: `https://localhost:5001/api/account/test-welcome-email`

- [ ] Page loads without errors
- [ ] Welcome template renders
- [ ] Branding looks good
- [ ] All text readable

### **5. Test Password Reset Flow**

Use Swagger or Postman:

**Request:**
```http
POST https://localhost:5001/api/account/forgot-password
Content-Type: application/json

{
  "email": "test@example.com"
}
```

**Expected Response:**
```json
{
  "message": "If an account exists with this email, a password reset link has been sent."
}
```

- [ ] Request succeeds (200 OK)
- [ ] Generic message returned (email enumeration prevented)
- [ ] Check console for email log

**Console Output Should Show:**
```
--- EMAIL SENT ---
To: test@example.com
Subject: Reset Your Password - School Management
Body: <!DOCTYPE html>...
------------------
```

- [ ] Console shows email details
- [ ] HTML starts with `<!DOCTYPE html>`

### **6. Test Registration Flow**

**Request:**
```http
POST https://localhost:5001/api/account/register
Content-Type: application/json

{
  "email": "newuser@example.com",
  "password": "SecurePass123!"
}
```

- [ ] Registration succeeds
- [ ] WelcomeEmail event published (check logs)
- [ ] Handler processes event

---

## 🔧 Configuration Phase

### **7. Choose Email Provider**

#### **Option A: SendGrid** ✅ Recommended

**Install:**
```bash
dotnet add SchoolManagement.Infrastructure package SendGrid
```

**Get API Key:**
1. Sign up: https://signup.sendgrid.com/
2. Navigate to: Settings → API Keys
3. Create new API key with "Mail Send" permission

**Update appsettings.json:**
```json
{
  "SendGrid": {
    "ApiKey": "SG.YOUR_API_KEY_HERE"
  },
  "Email": {
    "FromAddress": "noreply@yourschool.com",
    "FromName": "School Management System"
  }
}
```

- [ ] Package installed
- [ ] API key obtained
- [ ] Configuration added

**Update EmailService.cs:**
```csharp
private readonly ISendGridClient _sendGridClient;
private readonly string _fromAddress;
private readonly string _fromName;

public EmailService(
    EmailTemplateService templateService,
    ISendGridClient sendGridClient,
    IConfiguration configuration)
{
    _templateService = templateService;
    _sendGridClient = sendGridClient;
    _fromAddress = configuration["Email:FromAddress"];
    _fromName = configuration["Email:FromName"];
}

public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
{
    var from = new EmailAddress(_fromAddress, _fromName);
    var to = new EmailAddress(toEmail);
    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlBody);
    
    var response = await _sendGridClient.SendEmailAsync(msg);
    
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Failed to send email: {response.StatusCode}");
    }
}
```

**Register in Program.cs:**
```csharp
builder.Services.AddScoped<ISendGridClient>(_ => 
    new SendGridClient(builder.Configuration["SendGrid:ApiKey"]));
```

- [ ] EmailService updated
- [ ] SendGrid registered in DI

#### **Option B: SMTP** (Gmail, Office365)

**Install:**
```bash
dotnet add SchoolManagement.Infrastructure package MailKit
```

**Update appsettings.json:**
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true
  },
  "Email": {
    "FromAddress": "noreply@yourschool.com",
    "FromName": "School Management System"
  }
}
```

- [ ] Package installed
- [ ] SMTP credentials obtained
- [ ] Configuration added
- [ ] App password created (for Gmail)

---

## ✅ Production Verification

### **8. Send Real Test Email**

Use your actual email:

```http
POST https://localhost:5001/api/account/forgot-password
Content-Type: application/json

{
  "email": "your-real-email@gmail.com"
}
```

- [ ] Email received in inbox
- [ ] Template looks good on desktop
- [ ] Template looks good on mobile
- [ ] Reset button works
- [ ] Links are clickable
- [ ] Not in spam folder

### **9. Test Welcome Email**

Register with real email:

```http
POST https://localhost:5001/api/account/register
Content-Type: application/json

{
  "email": "your-real-email@gmail.com",
  "password": "SecurePass123!"
}
```

- [ ] Welcome email received
- [ ] Branding looks professional
- [ ] No broken images
- [ ] Content is clear

### **10. Email Client Testing**

Test emails in different clients:

- [ ] Gmail (web)
- [ ] Outlook (web)
- [ ] Apple Mail
- [ ] Mobile (iOS/Android)

---

## 🎨 Customization Phase

### **11. Brand Customization**

**Update Colors:**

Edit `.razor` files and change gradients:

```css
/* Password Reset - Current */
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

/* Your brand colors */
background: linear-gradient(135deg, #YOUR_COLOR1 0%, #YOUR_COLOR2 100%);
```

- [ ] Colors updated in all templates
- [ ] Test endpoints show new colors

**Add Logo:**

```html
<div class="header">
    <img src="https://yourschool.com/logo.png" alt="Logo" style="max-width: 150px; margin-bottom: 20px;">
    <h1>🔐 Password Reset Request</h1>
</div>
```

- [ ] Logo added
- [ ] Logo displays correctly
- [ ] HTTPS URL used

### **12. Update Content**

- [ ] Update support email/URL
- [ ] Update privacy policy link
- [ ] Update company name
- [ ] Update footer copyright

---

## 🌍 Localization (Optional)

### **13. Arabic Translation**

- [ ] Create `PasswordResetEmail.ar.razor`
- [ ] Test RTL layout
- [ ] Update EmailTemplateService with locale selection

### **14. French Translation**

- [ ] Create `PasswordResetEmail.fr.razor`
- [ ] Test content
- [ ] Update EmailTemplateService

---

## 📊 Monitoring Phase

### **15. Add Logging**

Update `EmailService.cs`:

```csharp
private readonly ILogger<EmailService> _logger;

public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
{
    try
    {
        _logger.LogInformation("Sending email to {Email} with subject {Subject}", toEmail, subject);
        
        // Send email...
        
        _logger.LogInformation("Email sent successfully to {Email}", toEmail);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
        throw;
    }
}
```

- [ ] Logger injected
- [ ] Success logs added
- [ ] Error logs added

### **16. Add Email Tracking**

- [ ] Track sent emails in database (optional)
- [ ] Monitor delivery rates
- [ ] Set up bounce/complaint handling

---

## 🚀 Deployment Checklist

### **17. Pre-Deployment**

- [ ] Email provider credentials in production `appsettings.json`
- [ ] FROM address verified (SPF/DKIM)
- [ ] Test emails in production environment
- [ ] Rate limiting configured
- [ ] Error handling tested

### **18. Post-Deployment**

- [ ] Send test password reset in production
- [ ] Verify emails arrive
- [ ] Check spam score (use mail-tester.com)
- [ ] Monitor email logs
- [ ] Set up alerts for failed emails

---

## 🎉 Final Verification

### **All Systems Go!**

- [ ] ✅ Templates render correctly
- [ ] ✅ Email provider configured
- [ ] ✅ Real emails sending
- [ ] ✅ Branding customized
- [ ] ✅ Logging implemented
- [ ] ✅ Tested in production
- [ ] ✅ Documentation reviewed
- [ ] ✅ Team trained on system

---

## 📝 Success Criteria

Your email system is **production-ready** when:

1. ✅ Test endpoints work without errors
2. ✅ Real emails arrive in inbox (not spam)
3. ✅ Templates look professional on all devices
4. ✅ Security info (IP, timestamp) is captured
5. ✅ Error handling works correctly
6. ✅ Logging shows all email activity
7. ✅ Performance is acceptable (< 2 seconds per email)

---

## 🆘 Troubleshooting

### **Emails not sending?**
→ Check: Provider credentials, firewall, logs

### **Templates not rendering?**
→ Check: `AddRazorComponents()` in Program.cs

### **Events not firing?**
→ Check: MediatR registration, handler namespace

### **Emails in spam?**
→ Check: SPF/DKIM records, sender reputation

---

## 🎯 Next Phase: Hangfire

Once MVP is stable, add Hangfire for invoice emails:

```bash
# Already installed!
dotnet add package Hangfire
```

Update `InvoiceGeneratedEventHandler.cs`:
```csharp
BackgroundJob.Enqueue<IEmailService>(x => 
    x.SendInvoiceEmailAsync(email, studentName, invoiceNumber, ...));
```

- [ ] Hangfire configured
- [ ] Invoice emails moved to background jobs
- [ ] Retry logic implemented

---

**🚀 You're ready to ship!**
