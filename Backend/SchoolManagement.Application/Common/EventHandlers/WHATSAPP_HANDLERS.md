# WhatsApp Event Handlers - Core Only

## ✅ Implemented (4 Essential Handlers)

### 1. EnrollmentCreatedWhatsAppHandler
**Trigger**: `EnrollmentCreatedDomainEvent`  
**When**: Student successfully enrolls  
**Message**: Welcome message with enrollment details

**Example:**
```
Welcome Ahmed! 🎉

You have been successfully enrolled in Mathematics.
Enrollment Date: 2026-08-05

We're excited to have you with us!

For any questions, feel free to contact us.
```

---

### 2. InvoiceCreatedWhatsAppHandler
**Trigger**: `InvoiceCreatedDomainEvent`  
**When**: Invoice is generated for student  
**Message**: Invoice details with amount and due date

**Example:**
```
Hello Ahmed,

Your invoice has been issued.

💰 Amount: 3,000.00 MAD
📅 Due Date: 2026-08-15
📋 Period: 2026-08-01 to 2026-08-31

Please proceed with payment before the due date.

Thank you! 🙏
```

---

### 3. PaymentReceivedWhatsAppHandler
**Trigger**: `PaymentReceivedDomainEvent`  
**When**: Payment is successfully received  
**Message**: Payment receipt confirmation

**Example:**
```
Payment Received ✅

Hello Ahmed,

Your payment has been successfully processed.

💰 Amount: 3,000.00 MAD
📅 Date: 2026-08-05 14:30
💳 Method: Cash
📋 Reference: 12ab34cd

Thank you for your payment! 🙏
```

---

### 4. InvoiceOverdueWhatsAppHandler
**Trigger**: `InvoiceOverdueDomainEvent`  
**When**: Invoice due date passes without full payment  
**Message**: Overdue reminder with amount due

**Example:**
```
⚠️ Payment Reminder

Hello Ahmed,

This is a friendly reminder that your invoice is now overdue.

💰 Amount Due: 3,000.00 MAD
📅 Due Date: 2026-08-15
📆 Overdue Since: 2026-08-16

Please make your payment as soon as possible to avoid any interruption in service.

Contact us if you need any assistance.
```

---

## 🚫 Not Implemented (Low Priority)

### ~~Enrollment Dropped~~
**Reason**: Administrative action - no WhatsApp needed

### ~~Enrollment Completed~~
**Reason**: End of course - can be handled manually if needed

### ~~Group Transfer~~
**Reason**: Internal operation - email/system notification sufficient

### ~~Invoice Waived~~
**Reason**: Administrative action - usually communicated in person

### ~~Invoice Cancelled~~
**Reason**: Rare occurrence - manual communication better

### ~~Overpayment (Credit Balance)~~
**Reason**: Positive event - student will know from payment receipt

---

## 📋 Domain Events Required

### ✅ Already Exists:
- `EnrollmentCreatedDomainEvent`
- `InvoiceOverdueDomainEvent`

### ⚠️ Need to Create:
- `InvoiceCreatedDomainEvent` - ✅ Created
- `PaymentReceivedDomainEvent` - ✅ Created

### 📝 Where to Raise Events:

#### Invoice.Create()
```csharp
public static Invoice Create(...)
{
    var invoice = new Invoice { ... };
    
    // ADD THIS:
    invoice.AddDomainEvent(new InvoiceCreatedDomainEvent(
        invoice.Id,
        enrollmentId,
        branchId,
        totalAmount: 0, // Will be set when charge is added
        dueDate
    ));
    
    return invoice;
}
```

#### Payment.Create() or InvoiceService.ReceivePayment()
```csharp
public async Task ReceivePaymentAsync(...)
{
    var payment = Payment.Create(...);
    invoice.AddPayment(payment);
    
    // ADD THIS:
    payment.AddDomainEvent(new PaymentReceivedDomainEvent(
        payment.Id,
        payment.InvoiceId,
        invoice.EnrollmentId,
        payment.Amount,
        payment.PaidAt
    ));
    
    await _repository.SaveAsync(payment);
}
```

---

## 🎯 Message Delivery Flow

```
1. Domain Event Raised (e.g., EnrollmentCreated)
   ↓
2. MediatR publishes event to all handlers
   ↓
3. WhatsApp Handler receives event
   ↓
4. Handler loads student phone number
   ↓
5. Handler queues message (INSERT into WhatsAppMessages)
   ↓
6. Handler returns immediately (non-blocking)
   ↓
7. Node.js worker picks up message (within 10s)
   ↓
8. Worker sends via WhatsApp Web
   ↓
9. Worker updates status (Sent/Failed)
```

---

## 🔧 Configuration

**Enable/Disable Handlers:**

Create configuration in `appsettings.json`:
```json
{
  "WhatsApp": {
    "Enabled": true,
    "EnableEnrollmentWelcome": true,
    "EnableInvoiceIssued": true,
    "EnablePaymentReceipt": true,
    "EnableOverdueReminder": true
  }
}
```

Then in handlers:
```csharp
public class EnrollmentCreatedWhatsAppHandler : INotificationHandler<EnrollmentCreatedDomainEvent>
{
    private readonly IConfiguration _config;
    
    public async Task Handle(EnrollmentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!_config.GetValue<bool>("WhatsApp:EnableEnrollmentWelcome"))
            return; // Skip if disabled
        
        // ... rest of handler
    }
}
```

---

## 📊 Monitoring

### Check Queued Messages
```sql
SELECT 
    MessageType,
    Status,
    COUNT(*) AS Count
FROM WhatsAppMessages
WHERE CreatedAt >= DATEADD(DAY, -7, GETUTCDATE())
GROUP BY MessageType, Status
ORDER BY MessageType, Status;
```

### Failed Messages by Event Type
```sql
SELECT 
    EntityType,
    COUNT(*) AS FailedCount
FROM WhatsAppMessages
WHERE Status = 3 -- Failed
  AND CreatedAt >= DATEADD(DAY, -1, GETUTCDATE())
GROUP BY EntityType
ORDER BY FailedCount DESC;
```

---

## 🧪 Testing

### Test Enrollment Welcome
```csharp
// Create student with phone
var student = await _studentService.CreateAsync(new StudentCommand
{
    FirstName = "Test",
    LastName = "Student",
    Phone = "0612345678", // Your test number
    ...
});

// Enroll student (triggers EnrollmentCreatedDomainEvent)
var enrollment = await _enrollmentService.CreateAsync(new EnrollmentCommand
{
    StudentId = student.Id,
    SubjectId = mathSubjectId,
    ...
});

// Check WhatsAppMessages table
// Message should appear within seconds
// Node.js worker will send within 10s
```

### Test Invoice Issued
```csharp
// Create invoice (triggers InvoiceCreatedDomainEvent)
var invoice = await _invoiceService.CreateAsync(new InvoiceCommand
{
    EnrollmentId = enrollmentId,
    DueDate = DateTime.UtcNow.AddDays(7),
    ...
});

// Check WhatsAppMessages table for InvoiceIssued message
```

---

## ⚠️ Important Notes

1. **Non-Blocking**: All handlers queue messages and return immediately - never block domain operations
2. **Fault Tolerant**: If WhatsApp fails, message stays in queue for retry
3. **No Spam**: Each event triggers ONE message only (no duplicates)
4. **Phone Validation**: Messages only sent if student has valid phone number
5. **Error Logging**: All errors logged but don't interrupt business operations

---

## 🚀 Deployment Checklist

- [ ] Register handlers in DI (auto-registered via Scrutor)
- [ ] Add domain events to Invoice.Create() and Payment.Create()
- [ ] Run database migration for WhatsAppMessages table
- [ ] Start Node.js worker
- [ ] Scan WhatsApp QR code
- [ ] Test each handler with real enrollment/invoice/payment
- [ ] Monitor queue for 24 hours
- [ ] Set up alerting if queue grows >100 messages
