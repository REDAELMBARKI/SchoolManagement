# Current Tasks - Invoice & Enrollment Lifecycle Implementation

**Epic**: Implement Invoice Lifecycle & Settlement Engine + Enrollment Lifecycle Workflows

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

---

### Story 3: Invoice Cancellation Workflow
**Priority**: P0 - Critical  
**Story Points**: 3

**Tasks**:
- [ ] **DOM-11**: Add `CancelInvoice(string reason)` method to `Invoice` entity
  - Validate: Invoice must be in Pending or PartiallyPaid status
  - Transition status to `InvoiceStatus.Cancelled`
  - Add domain event `InvoiceCancelledDomainEvent`

- [ ] **APP-12**: Create `CancelInvoiceCommand` DTO
  - Properties: InvoiceId, Reason, CancelledByUserId

- [ ] **APP-13**: Add `CancelInvoiceAsync` method to `IInvoiceService`
- [ ] **APP-14**: Implement `CancelInvoiceAsync` in `InvoiceService`
  - Retrieve invoice
  - Call `invoice.CancelInvoice(command.Reason)`
  - Handle linked charges (mark as cancelled)
  - Save via repository
  - Log audit trail

- [ ] **APP-15**: Create `CancelInvoiceValidator`
- [ ] **API-16**: Add `POST /api/invoices/{id}/cancel` endpoint

---

### Story 4: Overpayment & Credit Balance Handling
**Priority**: P0 - Critical  
**Story Points**: 4

**Tasks**:
- [ ] **DOM-17**: Add `ApplyOverpaymentToCredit(decimal overpaymentAmount)` method to `Enrollment` entity
  - Validate: overpaymentAmount must be positive
  - Add to `CreditBalance`
  - Add domain event `CreditBalanceUpdatedDomainEvent`

- [ ] **DOM-18**: Modify `Invoice.AddPayment()` to detect overpayment
  - If payment > remaining balance, calculate overpayment
  - Return overpayment amount to caller
  - Emit `InvoiceOverpaymentDomainEvent`

- [ ] **APP-19**: Update `PaymentService` to handle overpayment
  - After adding payment to invoice, check for overpayment
  - If overpayment exists, call `enrollment.ApplyOverpaymentToCredit(overpayment)`

- [ ] **DOM-20**: Add `UseCreditBalance(decimal amount)` method to `Enrollment` entity
  - Validate: amount <= CreditBalance
  - Deduct from CreditBalance
  - Add domain event

- [ ] **APP-21**: Update invoice payment logic to check for available credit balance first
  - Before processing payment, check enrollment credit balance
  - Apply credit balance to invoice if available

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

---

### Story 12: Integration Tests for API Endpoints
**Priority**: P2 - Medium  
**Story Points**: 4

**Tasks**:
- [ ] **TEST-52**: Add integration tests for Invoice API endpoints
  - POST /api/invoices/{id}/waive
  - POST /api/invoices/{id}/cancel

- [ ] **TEST-53**: Add integration tests for Enrollment API endpoints
  - POST /api/enrollments/{id}/drop
  - POST /api/enrollments/{id}/complete
  - POST /api/enrollments/{id}/transfer

---

## Summary

**Total Stories**: 12  
**Total Tasks**: 53  
**Estimated Effort**: ~48 story points

**Implementation Order Recommendation**:
1. Start with Story 5 (Drop Enrollment) - foundational for enrollment lifecycle
2. Story 6 (Complete Enrollment) - simpler, builds on drop pattern
3. Story 1 (Overdue Processor) - independent background service
4. Story 2 (Invoice Waiver) - foundational for invoice lifecycle
5. Story 3 (Invoice Cancellation) - builds on waiver pattern
6. Story 4 (Overpayment Handling) - integrates with payment flow
7. Story 7 (Group Transfer) - most complex, requires capacity + clash detection
8. Story 8 (Atomic Capacity) - infrastructure improvement for transfer
9. Story 9-10 (Cross-cutting) - can be done in parallel with domain stories
10. Story 11-12 (Testing) - ongoing throughout implementation
