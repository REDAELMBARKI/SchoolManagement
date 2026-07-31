# Current Tasks - Invoice & Enrollment Lifecycle Implementation

**Epic**: Implement Invoice Lifecycle & Settlement Engine + Enrollment Lifecycle Workflows

**Last updated**: Story 3 complete. Story 4 partially started (config + renewal/cancel credit; payment overpayment still pending).

---

## Invoice Lifecycle & Settlement Engine

### Story 1: Automated Overdue Invoice Background Service
**Priority**: P0 - Critical  
**Story Points**: 5

**Tasks**:
- [ ] **INF-1**: Create background service `OverdueInvoiceProcessor` in `SchoolManagement.Infrastructure/Services/`
  - Implement `ExecuteAsync` method to run daily
  - Query invoices where `DueDate < DateTime.UtcNow` AND `PaidAmount < TotalAmount` AND `Status != Overdue`
  - Call `invoice.RecalculateStatus()` to transition to `InvoiceStatus.Overdue`
  - Add audit logging for each transition

- [ ] **DOM-2**: Add domain event `InvoiceOverdueDomainEvent` in `SchoolManagement.Domain/DomainEvents/`
  - Include properties: InvoiceId, EnrollmentId, OverdueDate, AmountDue
  - Emit event when status transitions to Overdue ;

- [ ] **APP-3**: Create event handler `InvoiceOverdueNotificationHandler` in `SchoolManagement.Application/EventsHandlers/`
  - Handle `InvoiceOverdueDomainEvent`
  - Send notification to student/parent (placeholder for email/SMS integration)

- [ ] **API-4**: Register background service in `Program.cs`
  - Add `builder.Services.AddHostedService<OverdueInvoiceProcessor>()`

> **Partial (Hangfire)**: `InvoiceService.ProcessPastDueInvoicesAsync()` + Hangfire recurring job exist. Still missing domain event, audit per transition, and dedicated hosted service from spec.

---

### Story 2: Invoice Waiver Workflow
**Priority**: P0 - Critical  
**Story Points**: 3

**Tasks**:
- [x] **DOM-5**: Add `WaiveInvoice(decimal waivedAmount, string reason)` method to `Invoice` entity
  - Validate: waivedAmount must be positive and <= remaining balance (TotalAmount - PaidAmount)
  - Reduce TotalAmount by waivedAmount (or add negative charge)
  - Recalculate status after waiver
  - Add domain event `InvoiceWaivedDomainEvent`

- [x] **APP-6**: Create `WaiveInvoiceCommand` DTO in `SchoolManagement.Application/Dtos/`
  - Properties: InvoiceId, WaivedAmount, Reason, WaivedByUserId

- [x] **APP-7**: Add `WaiveInvoiceAsync` method to `IInvoiceService` interface
- [x] **APP-8**: Implement `WaiveInvoiceAsync` in `InvoiceService`
  - Retrieve invoice by ID
  - Call `invoice.WaiveInvoice(command.WaivedAmount, command.Reason)`
  - Save via repository
  - Log audit trail

- [x] **APP-9**: Create `WaiveInvoiceValidator` using FluentValidation
- [x] **API-10**: Add `POST /api/invoices/{id}/waive` endpoint in `InvoiceController`

- [x] **INF-5**: `InvoiceRepository.GetByIdAsync` includes `Charges` (required for waive/cancel charge handling)

---

### Story 3: Invoice Cancellation Workflow
**Priority**: P0 - Critical  
**Story Points**: 3  
**Status**: ✅ Done

**Tasks**:
- [x] **DOM-11**: Add `CancelInvoice(string reason)` method to `Invoice` entity
  - Validate: Invoice must be in Pending or PartiallyPaid status
  - Transition status to `InvoiceStatus.Cancelled`
  - Cancel all active linked charges
  - Add domain event `InvoiceCancelledDomainEvent`

- [x] **APP-12**: Create `CancelInvoiceCommand` DTO
  - Properties: InvoiceId, Reason, CancelledByUserId

- [x] **APP-13**: Add `CancelInvoiceAsync` method to `IInvoiceService`
- [x] **APP-14**: Implement `CancelInvoiceAsync` in `InvoiceService`
  - Retrieve invoice (with charges)
  - Call `invoice.CancelInvoice(command.Reason)`
  - Handle linked charges (mark as cancelled)
  - Save via repository
  - Log audit trail (`AuditLog.CancelAction`)

- [x] **APP-15**: Create `CancelInvoiceValidator` (validates `CancelInvoiceCommand`)
- [x] **API-16**: Add `POST /api/invoices/{id}/cancel` endpoint



### Story 4: Overpayment & Credit Balance Handling
**Priority**: P0 - Critical  
**Story Points**: 4  
**Status**: 🟡 In progress (~40%)

**Decisions locked in**:
- Allow overpayment → spill to `Enrollment.CreditBalance` (prepay for future invoices)
- Auto-apply credit **on renewal invoices only** — not on manual payments
- Use `AddCredit()` + `UseCredit()` on enrollment (no separate `ApplyOverpaymentToCredit` / `UseCreditBalance` names)
- One active charge per billing invoice (convention; config `MaxActiveChargesPerInvoice`)
- On cancel: restore credit using configurable **percentage** + time rules (not cash refunds)

**Tasks**:
- [x] **DOM-17**: Enrollment credit increase — use existing `AddCredit(decimal amount)` (supersedes `ApplyOverpaymentToCredit`)
  - Used in registration overpayment + cancel restore


- [ ] **DOM-18**: Modify `Invoice.AddPayment()` to detect overpayment
  - Cap payment at remaining balance; return overpayment to caller
  - Allocate payment to active charges (charge `PaidAmount` sync)
  - Emit `InvoiceOverpaymentDomainEvent`

- [ ] **APP-19**: Update `PaymentService` to handle overpayment
  - After applying payment to invoice, route overpayment to `enrollment.AddCredit()` when `BillingOptions.AllowOverpaymentToCredit` is true

- [x] **DOM-20**: Add `UseCredit(decimal amount)` to `Enrollment` entity
  - Validates amount > 0 and <= `CreditBalance`
  - [ ] Add domain event (deferred to Story 10)

- [x] **APP-21**: Apply credit on renewal invoice generation only
  - `GenerateDailyInvoicesAsync`: `UseCredit` + `Invoice.RecordCreditApplied`
  - Controlled by `BillingOptions.ApplyCreditOnRenewalOnly`
  - Manual invoice payments do **not** auto-consume credit (by design)

- [x] **DOM-21**: Add `Invoice.CreditAppliedAmount` + `RecordCreditApplied(decimal)`
  - Tracks enrollment credit consumed on this invoice (for cancel restore)

- [x] **APP-22**: Create `BillingOptions` + `appsettings.json` `"Billing"` section
  - `AllowOverpaymentToCredit`, `ApplyCreditOnRenewalOnly`, `CreditRestorePercentage`
  - `RestoreCreditBeforePeriodStartOnly`, `GracePeriodDaysAfterPeriodStart`
  - `MaxActiveChargesPerInvoice`
  - Registered in `Program.cs` via `IOptions<BillingOptions>`

- [x] **APP-23**: Restore credit on invoice cancel (`CancelInvoiceAsync`)
  - `restore = CreditAppliedAmount × CreditRestorePercentage / 100`
  - Gated by period start + grace period from config

- [x] **APP-24**: Registration prepayment — `StudentRegistrationService` stores overpayment via `AddCreditAsync` when `amountPaid > plan.Amount`

- [ ] **DOM-25**: `Charge.AddPayment(decimal)` + single `Invoice.ApplyPayment` entry point (charge/invoice paid amount sync)

> **Not in scope (explicit)**: Auto-apply credit when staff records a manual payment — renewals only.

---

## Enrollment Lifecycle Workflows

### Story 5: Drop Enrollment Workflow
**Priority**: P0 - Critical  
**Story Points**: 5

**Tasks**:
- [ ] **DOM-22**: Add `DropEnrollment(string reason, DateTime? droppedAt = null)` method to `Enrollment` entity
  - Validate: Current status must be Active
  - Transition status to `EnrollmentStatus.Dropped`
  - Set dropped date
  - Add domain event `EnrollmentDroppedDomainEvent`

- [ ] **DOM-23**: Add `ReleaseGroupCapacity()` method to `Group` entity
  - Decrement effective capacity or recalculate available space
  - This should be called when enrollment is dropped

- [ ] **APP-24**: Create `DropEnrollmentCommand` DTO
  - Properties: EnrollmentId, Reason, DroppedByUserId

- [ ] **APP-25**: Add `DropEnrollmentAsync` method to `IEnrollmentService`
- [ ] **APP-26**: Implement `DropEnrollmentAsync` in `EnrollmentService`
  - Retrieve enrollment with Group
  - Call `enrollment.DropEnrollment(command.Reason)`
  - Call `group.ReleaseGroupCapacity()` (or handle via Group's capacity recalculation)
  - Determine drop policy (cancel invoice vs retain liability)
  - If policy requires cancellation, call invoice cancellation logic
  - Save via repository
  - Log audit trail

- [ ] **APP-27**: Create `DropEnrollmentValidator`
- [ ] **API-28**: Add `POST /api/enrollments/{id}/drop` endpoint

---

### Story 6: Complete Enrollment Workflow
**Priority**: P0 - Critical  
**Story Points**: 3

**Tasks**:
- [ ] **DOM-29**: Add `CompleteEnrollment(string? notes = null)` method to `Enrollment` entity
  - Validate: Current status must be Active
  - Transition status to `EnrollmentStatus.Completed`
  - Set completion date
  - Lock further fee modifications (add validation flag)
  - Add domain event `EnrollmentCompletedDomainEvent`

- [ ] **APP-30**: Create `CompleteEnrollmentCommand` DTO
  - Properties: EnrollmentId, Notes, CompletedByUserId

- [ ] **APP-31**: Add `CompleteEnrollmentAsync` method to `IEnrollmentService`
- [ ] **APP-32**: Implement `CompleteEnrollmentAsync` in `EnrollmentService`
  - Retrieve enrollment
  - Call `enrollment.CompleteEnrollment(command.Notes)`
  - Save via repository
  - Log audit trail

- [ ] **APP-33**: Create `CompleteEnrollmentValidator`
- [ ] **API-34**: Add `POST /api/enrollments/{id}/complete` endpoint

---

### Story 7: Group Transfer Workflow
**Priority**: P0 - Critical  
**Story Points**: 8

**Tasks**:
- [ ] **DOM-35**: Add `TransferGroup(Guid newGroupId, string? reason = null)` method to `Enrollment` entity
  - Validate: Current status must be Active
  - Validate: newGroupId != current GroupId
  - Add domain event `EnrollmentTransferRequestedDomainEvent`

- [ ] **DOM-36**: Add capacity validation logic to `Group` entity
  - Add `HasCapacityForAdditionalEnrollment()` method
  - Consider concurrency handling (add row version or capacity lock)

- [ ] **DOM-37**: Add schedule clash detection helper in `Schedule` entity or separate service
  - Method: `HasScheduleConflict(Guid studentId, Guid newGroupId, DateTime startDate, DateTime endDate)`
  - Query student's other active enrollments
  - Check for overlapping time slots

- [ ] **APP-38**: Create `TransferGroupCommand` DTO
  - Properties: EnrollmentId, NewGroupId, Reason, TransferredByUserId

- [ ] **APP-39**: Add `TransferGroupAsync` method to `IEnrollmentService`
- [ ] **APP-40**: Implement `TransferGroupAsync` in `EnrollmentService`
  - Retrieve enrollment with current Group
  - Retrieve new Group with Level and Subject
  - Validate: new Group has same Level and Subject as current Group
  - Validate: new Group has available capacity (atomic check)
  - Validate: no schedule clashes for student
  - Call `enrollment.TransferGroup(command.NewGroupId, command.Reason)`
  - Update GroupId
  - Handle capacity: increment new group, decrement old group
  - Save via repository (consider transaction)
  - Log audit trail

- [ ] **APP-41**: Create `TransferGroupValidator`
- [ ] **API-42**: Add `POST /api/enrollments/{id}/transfer` endpoint

---

### Story 8: Atomic Capacity Guard with Concurrency
**Priority**: P0 - Critical  
**Story Points**: 4

**Tasks**:
- [ ] **DOM-43**: Add `RowVersion` (timestamp) to `Group` entity for optimistic concurrency
  - Add property: `public byte[] RowVersion { get; set; }`
  - Configure in EF Core `GroupConfiguration`

- [ ] **INF-44**: Update `GroupConfiguration` to configure RowVersion as concurrency token
  - `builder.Property(g => g.RowVersion).IsRowVersion().IsConcurrencyToken()`

- [ ] **APP-45**: Update enrollment creation and transfer logic to handle concurrency
  - Wrap capacity check and update in transaction
  - Catch `DbUpdateConcurrencyException` and retry or fail gracefully

- [ ] **INF-46**: Add integration test for concurrent enrollment creation
  - Test: Two threads try to enroll in same group at capacity limit
  - Verify: Only one succeeds, other gets concurrency error

---

## Cross-Cutting Concerns

### Story 9: Audit Logging for New Workflows
**Priority**: P1 - Important  
**Story Points**: 2

**Tasks**:
- [ ] **APP-47**: Ensure all new service methods use `IAuditLogService`
  - Invoice waiver, cancellation, overdue transitions
  - Enrollment drop, complete, transfer
  - Log: Action, EntityId, OldValues, NewValues, UserId, Timestamp

---

### Story 10: Domain Events Integration
**Priority**: P1 - Important  
**Story Points**: 2

**Tasks**:
- [ ] **APP-48**: Register all new domain events in MediatR
  - InvoiceOverdueDomainEvent
  - InvoiceWaivedDomainEvent
  - InvoiceCancelledDomainEvent
  - InvoiceOverpaymentDomainEvent
  - EnrollmentDroppedDomainEvent
  - EnrollmentCompletedDomainEvent
  - EnrollmentTransferRequestedDomainEvent
  - CreditBalanceUpdatedDomainEvent

---

## Testing

### Story 11: Unit Tests for Domain Logic
**Priority**: P1 - Important  
**Story Points**: 5

**Tasks**:
- [ ] **TEST-49**: Add unit tests for Invoice domain methods
  - `WaiveInvoice` - test waiver amount validation, status recalculation
  - `CancelInvoice` - test status validation, transition
  - `AddPayment` - test overpayment detection

- [ ] **TEST-50**: Add unit tests for Enrollment domain methods
  - `DropEnrollment` - test status validation, transition
  - `CompleteEnrollment` - test status validation, lock flag
  - `TransferGroup` - test validation logic

- [ ] **TEST-51**: Add unit tests for Group capacity logic
  - `HasAvailableSpace` - test with various enrollment states
  - `GetRemainingCapacity` - test calculation accuracy

