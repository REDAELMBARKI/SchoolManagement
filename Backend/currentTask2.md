# Current Tasks 2 - Remaining Unimplemented Features

**Based on analysis of current codebase vs nextTasks.md requirements**

---

## ✅ Already Implemented (from currentTask.md)

### Invoice Lifecycle - COMPLETE
- ✅ Invoice.WaiveInvoice() domain method
- ✅ Invoice.CancelInvoice() domain method  
- ✅ Invoice.AddPayment() with overpayment detection
- ✅ Charge.Waive() and Charge.Cancel() methods
- ✅ InvoiceService.WaiveInvoiceAsync() and CancelInvoiceAsync()
- ✅ InvoiceController waive/cancel endpoints
- ✅ InvoiceService.ProcessPastDueInvoicesAsync() background job
- ✅ Domain events: InvoiceWaivedDomainEvent, InvoiceCancelledDomainEvent, InvoiceOverpaymentDomainEvent

### Enrollment Lifecycle - COMPLETE
- ✅ Enrollment.DropEnrollment() domain method
- ✅ Enrollment.CompleteEnrollment() domain method
- ✅ EnrollmentService.DropEnrollmentAsync() and CompleteEnrollmentAsync()
- ✅ EnrollmentController drop/complete endpoints
- ✅ Group.HasAvailableSpace() and capacity checks
- ✅ Group.TouchCapacityGuard() for concurrency
- ✅ Domain events: EnrollmentDroppedDomainEvent, EnrollmentCompletedDomainEvent
- ✅ Atomic capacity guard with optimistic concurrency

---

## ❌ Remaining P0 Critical Workflows

### Story 3: Invoice Overdue Notification System
**Priority**: P0 - Critical
**Story Points**: 3
**Status**: ✅ Done

**Tasks**:
- [x] **DOM-1**: Add `InvoiceOverdueDomainEvent` to DomainEvents
- [x] **APP-2**: Modify Invoice.RecalculateStatus() to emit InvoiceOverdueDomainEvent when status transitions to PastDue
- [x] **APP-3**: Create `InvoiceOverdueNotificationHandler` in Application/EventsHandlers/
- [x] **APP-4**: Register handler in MediatR pipeline
- [x] **API-5**: Verify OverdueInvoiceProcessor background service is registered in Program.cs


### Story 5: Commission Calculation Engine
**Priority**: P0 - Critical  
**Story Points**: 5

**Current State**: Commission entity doesn't exist

**Tasks**:
- [ ] **DOM-24**: Create Commission entity with full properties
- [ ] **DOM-25**: Add CommissionRepository interface
- [ ] **INF-26**: Implement CommissionRepository in Infrastructure
- [ ] **INF-27**: Add Commission EF Core configuration
- [ ] **APP-28**: Create `CommissionService`
  - CalculateCommission(enrollment, agent, commissionRate)
  - RecordCommission()
  - ProcessCommissionClawback(enrollmentId, reason)

- [ ] **APP-29**: Integrate commission calculation into:
  - Intake conversion pipeline
  - Payment processing (when payments are made)

---

### Story 6: Commission Clawback Rules
**Priority**: P0 - Critical  
**Story Points**: 4

**Tasks**:
- [ ] **DOM-30**: Add `ClawbackCommission(string reason)` method to Commission entity
  - Validate: Commission status is Earned
  - Transition status to ClawedBack
  - Add domain event

- [ ] **APP-31**: Modify Enrollment.DropEnrollment() to trigger commission clawback
  - Check if enrollment dropped within N-day trial period (e.g., 14 days)
  - If yes, find related commission and call clawback
  - Add domain event `CommissionClawbackDomainEvent`

- [ ] **APP-32**: Create `CommissionClawbackHandler` to handle clawback events
- [ ] **TEST-33**: Add unit tests for clawback logic

---


---

### Story 7: Payment Reversal & Refund Workflow
**Priority**: P0 - Critical  
**Story Points**: 5

**Current State**: PaymentService.DeleteAsync does hard deletion without reversal

**Tasks**:
- [ ] **DOM-59**: Add `RefundPayment(refundAmount, reason, method)` method to Payment entity
  - Validate: Payment exists and has sufficient amount
  - Add domain event `PaymentRefundedDomainEvent`

- [ ] **APP-60**: Replace PaymentService.DeleteAsync with `RefundPaymentAsync()`
  - Retrieve payment with Invoice and Enrollment
  - Call payment.RefundPayment()
  - Deduct refundAmount from Invoice.PaidAmount
  - Recalculate Invoice.Status
  - If payment was applied as credit, reverse Enrollment.CreditBalance
  - Save via transaction
  - Log audit trail

- [ ] **APP-61**: Create `RefundPaymentCommand` DTO
- [ ] **APP-62**: Create `RefundPaymentValidator`
- [ ] **API-63**: Add `POST /api/payments/{id}/refund` endpoint in PaymentController

---

### Story 8: Multi-Level Expense Approval Pipeline
**Priority**: P0 - Critical  
**Story Points**: 6

**Current State**: Expense entity exists but lacks approval workflow

**Tasks**:
- [ ] **DOM-64**: Add `SubmitExpense(requestedByUserId)` method to Expense entity
  - Set Status = Pending
  - Populate RequestedBy
  - Add domain event `ExpenseSubmittedDomainEvent`

- [ ] **DOM-65**: Add `ApproveExpense(approverUserId)` method to Expense entity
  - Validate: Approver != Requester
  - Validate: Approver has FinanceApprover role (via domain service)
  - Set Status = Approved
  - Populate ApprovedBy
  - Add domain event `ExpenseApprovedDomainEvent`

- [ ] **DOM-66**: Add `RejectExpense(approverUserId, reason)` method to Expense entity
  - Validate: Approver != Requester
  - Set Status = Rejected
  - Populate ApprovedBy
  - Add domain event `ExpenseRejectedDomainEvent`

- [ ] **DOM-67**: Add `MarkAsPaid(paymentMethod, referenceCode, paidAt)` method to Expense entity
  - Validate: Status is Approved
  - Set Status = Paid
  - Populate payment details
  - Add domain event `ExpensePaidDomainEvent`

- [ ] **APP-68**: Create `ExpenseService` in Application/Core/Services/
- [ ] **APP-69**: Implement `SubmitExpenseAsync()`, `ApproveExpenseAsync()`, `RejectExpenseAsync()`, `MarkExpensePaidAsync()`
- [ ] **APP-70**: Create ExpenseCommand DTOs
- [ ] **APP-71**: Create ExpenseValidators
- [ ] **API-72**: Create `ExpenseController`
  - POST /api/expenses/submit
  - POST /api/expenses/{id}/approve
  - POST /api/expenses/{id}/reject
  - POST /api/expenses/{id}/mark-paid

- [ ] **INF-73**: Create IExpenseRepository interface (if not exists)
- [ ] **INF-74**: Implement ExpenseRepository in Infrastructure (if not exists)

---

## ❌ Remaining P1 Important Workflows

### Story 9: Payment Plan Discount & Fee Evaluation
**Priority**: P1 - Important  
**Story Points**: 4

**Tasks**:
- [ ] **DOM-75**: Add `CalculateNetPayable()` method to Plan entity
  - Formula: NetPayable = BaseAmount × (1 − DiscountPercent / 100)
  - Support custom manager discount overrides

- [ ] **DOM-76**: Add `GenerateInstallmentSchedule()` method to Plan entity
  - Generate scheduled charges based on plan frequency (Monthly, Term, Annual)
  - Use Plan.RemainingAmountDueDays for due date calculation

- [ ] **APP-77**: Update invoice generation to use dynamic due dates
- [ ] **APP-78**: Add discount override audit logging

---


---


---

### Story 10: Media Polymorphic Ownership & Storage Governance
**Priority**: P1 - Important  
**Story Points**: 3

**Tasks**:
- [ ] **DOM-93**: Add `ValidateOwner()` method to Media entity
  - Validate OwnerId exists in corresponding repository (Student, Teacher, Branch, DomainUser)

- [ ] **APP-94**: Add storage quota validation
  - Validate file extensions, MIME types, max sizes per owner category

- [ ] **APP-95**: Integrate validation into MediaService

---

## ❌ Remaining P2 Reporting & Analytics

### Story 11: Financial Reporting & Aging Analysis
**Priority**: P2 - Medium  
**Story Points**: 6

**Tasks**:
- [ ] **APP-96**: Create `FinancialReportingService`
  - Revenue breakdown by Branch, Payment Plan
  - Charge & Invoice aging reports (0-30, 31-60, 61-90, 90+ days)
  - Branch profitability (Revenue - Expenses per month)

- [ ] **API-97**: Add financial reporting endpoints

---

### Story 12: Operational Analytics & Lead Funnel
**Priority**: P2 - Medium  
**Story Points**: 5

**Tasks**:
- [ ] **APP-98**: Create `OperationalAnalyticsService`
  - Group occupancy rate (EnrolledCount / Capacity)
  - Lead conversion funnel (New → Contacted → Interested → Converted)
  - Student retention rates

- [ ] **API-99**: Add analytics endpoints

---

### Story 13: Automated Background Jobs & Notifications
**Priority**: P2 - Medium  
**Story Points**: 4

**Tasks**:
- [ ] **INF-100**: Create `LeadFollowUpReminderProcessor` background service
- [ ] **INF-101**: Create event-driven notification service
  - Email/SMS templates for Invoice issued, Payment receipt, Lead follow-up
- [ ] **INF-102**: Register background services in Program.cs



### Story 14: Group Transfer Workflow
**Priority**: P0 - Critical  
**Story Points**: 8

**Current State**: Not implemented at all

**Tasks**:
- [ ] **DOM-9**: Add `TransferGroup(Guid newGroupId, string? reason)` method to Enrollment entity
  - Validate: Current status must be Active
  - Validate: newGroupId != current GroupId
  - Add domain event `EnrollmentTransferRequestedDomainEvent`

- [ ] **DOM-10**: Add schedule clash detection helper
  - Create `Schedule.HasScheduleConflict(Guid studentId, Guid newGroupId)` method
  - Query student's other active enrollments
  - Check for overlapping time slots

- [ ] **APP-11**: Create `TransferGroupCommand` DTO
  - Properties: EnrollmentId, NewGroupId, Reason, TransferredByUserId

- [ ] **APP-12**: Add `TransferGroupAsync` method to IEnrollmentService interface
- [ ] **APP-13**: Implement `TransferGroupAsync` in EnrollmentService
  - Retrieve enrollment with current Group and Schedule
  - Retrieve new Group with Level, Subject, and Schedule
  - Validate: new Group has same Level and Subject as current Group
  - Validate: new Group has available capacity (atomic check)
  - Validate: no schedule clashes for student
  - Call enrollment.TransferGroup()
  - Update GroupId
  - Handle capacity: increment new group, decrement old group
  - Save via repository with transaction
  - Log audit trail

- [ ] **APP-14**: Create `TransferGroupValidator` using FluentValidation
- [ ] **API-15**: Add `POST /api/enrollments/{id}/transfer` endpoint in EnrollmentController