# Domain Events - What We Should Raise

## ✅ Core Events Currently Raised

### **1. Registration Flow**
- ✅ **EnrollmentCreatedDomainEvent** - Raised when student enrolls in a course
  - Location: `Enrollment.Create()`
  - WhatsApp Handler: `EnrollmentCreatedWhatsAppHandler` ✅

---

### **2. Financial Flow - Invoices**
- ✅ **InvoiceCreatedDomainEvent** - Raised when invoice is issued
  - Location: `Invoice.Create()`
  - WhatsApp Handler: `InvoiceCreatedWhatsAppHandler` ✅
  - **Note:** Handler fetches TotalAmount from DB (minimal event pattern)

- ✅ **InvoiceOverdueDomainEvent** - Raised when invoice becomes past due
  - Location: `Invoice.RecalculateStatus()`
  - WhatsApp Handler: `InvoiceOverdueWhatsAppHandler` ✅

- ✅ **InvoiceWaivedDomainEvent** - Raised when invoice is waived
  - Location: `Invoice.WaiveInvoice()`
  - WhatsApp Handler: ❌ **MISSING** ⚠️

- ✅ **InvoiceCancelledDomainEvent** - Raised when invoice is cancelled
  - Location: `Invoice.CancelInvoice()`
  - WhatsApp Handler: ❌ **MISSING** ⚠️

- ✅ **InvoiceOverpaymentDomainEvent** - Raised when payment exceeds invoice amount
  - Location: `Invoice.AddPayment()`
  - WhatsApp Handler: ❌ **Not needed** (creates credit automatically)

---

### **3. Financial Flow - Payments**
- ✅ **PaymentReceivedDomainEvent** - Raised when payment is received
  - Location: `Payment.Create()` (when status = Paid)
  - WhatsApp Handler: `PaymentReceivedWhatsAppHandler` ✅
  - **Note:** Handler fetches invoice details from DB (minimal event pattern)

---

### **4. Enrollment Status Changes**
- ✅ **EnrollmentDroppedDomainEvent** - Raised when student drops enrollment
  - Location: `Enrollment.DropEnrollment()`
  - WhatsApp Handler: ❌ **MISSING** ⚠️

- ✅ **EnrollmentCompletedDomainEvent** - Raised when student completes course
  - Location: `Enrollment.CompleteEnrollment()`
  - WhatsApp Handler: ❌ **MISSING** ⚠️

- ✅ **EnrollmentGroupTransferredDomainEvent** - Raised when student moves to different group
  - Location: `Enrollment.TransferToGroup()`
  - WhatsApp Handler: ❌ **MISSING** ⚠️

---

### **5. Internal/System Events**
- ✅ **PayrollPaidDomainEvent** - Internal HR event (no WhatsApp needed)
  - Location: `Payroll` entity
  - WhatsApp Handler: ❌ Not needed (internal)

---

## ❌ Events REMOVED (Duplicates/Unused)

- ❌ **StudentCreatedDomainEvent** - Removed (use EnrollmentCreated instead)
  - Reason: Student creation alone doesn't mean enrollment
  - Use EnrollmentCreated for welcome messages

- ❌ **NewStudentAssignedDomainEvent** - Removed (duplicate of EnrollmentCreated)
  - Reason: Same as enrollment creation
  - Old handlers commented out in:
    - `SendWelcomeEmailHandler.cs` (empty implementation)
    - `UpdateIntakeStatusHandler.cs` (TODO: move to EnrollmentCreated)

---

## 🎯 Events Summary by Use Case

### **For Students/Parents (WhatsApp):**
| Event | Status | Priority |
|-------|--------|----------|
| EnrollmentCreated | ✅ Handler exists | Core |
| InvoiceCreated | ✅ Handler exists | Core |
| PaymentReceived | ✅ Handler exists | Core |
| InvoiceOverdue | ✅ Handler exists | Core |
| InvoiceWaived | ❌ Handler missing | High |
| InvoiceCancelled | ❌ Handler missing | High |
| EnrollmentDropped | ❌ Handler missing | Medium |
| EnrollmentCompleted | ❌ Handler missing | Medium |
| EnrollmentGroupTransferred | ❌ Handler missing | Medium |

### **Internal/System:**
- InvoiceOverpayment (creates credit - no notification needed)
- PayrollPaid (HR only - no notification needed)

---

## 🔥 Next Steps - Missing WhatsApp Handlers

### High Priority:
1. **InvoiceWaivedWhatsAppHandler** - Good news for students!
2. **InvoiceCancelledWhatsAppHandler** - Avoid confusion

### Medium Priority:
3. **EnrollmentDroppedWhatsAppHandler** - Important status change
4. **EnrollmentCompletedWhatsAppHandler** - Celebration message
5. **EnrollmentGroupTransferredWhatsAppHandler** - Schedule change notification

---

## 📋 Event Pattern (DDD Best Practices)

### ✅ Correct Pattern:
1. **Raise in Domain Entity** (Factory/Method)
   ```csharp
   public static Invoice Create(...)
   {
       var invoice = new Invoice { ... };
       invoice.AddDomainEvent(new InvoiceCreatedDomainEvent(...));
       return invoice;
   }
   ```

2. **Minimal Event Data** (IDs only)
   ```csharp
   public class InvoiceCreatedDomainEvent
   {
       public Guid InvoiceId { get; }
       public Guid EnrollmentId { get; }
       public Guid BranchId { get; }
       public DateTime DueDate { get; }
       // NO TotalAmount - handler will fetch it
   }
   ```

3. **Handler Fetches Details**
   ```csharp
   public async Task Handle(InvoiceCreatedDomainEvent evt, ...)
   {
       // Fetch fresh data from DB
       var invoice = await _invoiceQueryService.GetByIdAsync(evt.InvoiceId);
       var student = await _studentQueryService.GetByIdAsync(invoice.Enrollment.StudentId);
       
       // Now build message with all data
       await _whatsAppService.QueueMessageAsync(...);
   }
   ```

### ❌ Wrong Patterns:
- ❌ Raise events in service layer after DB save
- ❌ Put all data in event (TotalAmount, StudentName, etc.)
- ❌ Raise events for every entity creation (use meaningful business events)
