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

## ✅ Story 1: Invoice Overdue Notification System - COMPLETE
- ✅ InvoiceOverdueDomainEvent
- ✅ Invoice.RecalculateStatus() emits InvoiceOverdueDomainEvent on PastDue transition
- ✅ InvoiceOverdueNotificationHandler
- ✅ Registered in MediatR pipeline
- ✅ OverdueInvoiceProcessor background service registered in Program.cs

---

## ✅ Story 5 & 6: Commission System - COMPLETE

### Design Decisions
- **OPC**: flat commission per enrollment (amount from appsettings), created as `Approved` immediately
- **Commercial Agent**: tiered monthly commission (tiers from appsettings), calculated on 1st of month
- **Salary lockout**: day 13 at 8pm UTC (configurable in appsettings) — all `Approved` → `Paid`, `Blocked` stays `Blocked`
- **No Pending status** — commissions are either Approved, Blocked, or Paid

### Commission Status Lifecycle
```
Enrollment created (Active) → Commission: Approved  (immediately, via OpcCommissionHandler)
Enrollment dropped          → Commission: Blocked   (immediately, via EnrollmentDroppedCommissionHandler)
Salary day job (day 13 8pm) → Approved → Paid       (Hangfire, cron from appsettings)
After lockout               → frozen, nothing changes
```

### What was built
- ✅ `CommissionStatus` enum: Approved, Blocked, Paid
- ✅ `EarnerType` enum: Opc, CommercialAgent
- ✅ `Commission` entity (AggregateRoot): `CreateForOpc()`, `CreateForAgent()`, `Block(reason)`, `MarkAsPaid()`
- ✅ `ICommissionRepository` with custom queries (by earner, period, enrollment, agent monthly count)
- ✅ `CommissionRepository` implementing full chain: `Enrollment → Student → Intake → CommercialAgentId`
- ✅ `CommissionConfiguration` EF Core config + registered in `AppDbContext`
- ✅ `CommissionSettings` options class (OpcFlatAmount, SalaryDayOfMonth, SalaryLockoutHour, AgentTiers)
- ✅ `appsettings.json` Commission section with defaults
- ✅ `EnrollmentCreatedDomainEvent` — raised in `Enrollment.Create()`
- ✅ `OpcCommissionHandler` — fires on enrollment created, creates OPC commission
- ✅ `EnrollmentDroppedCommissionHandler` — fires on enrollment dropped, blocks OPC commission
- ✅ `EnrollmentService` — now publishes domain events after CreateAsync and DropEnrollmentAsync
- ✅ `CommissionService`: `ProcessOpcCommissionAsync`, `ProcessAgentMonthlyCommissionsAsync`, `ProcessSalaryLockoutAsync`, `BlockCommissionAsync`, `BlockOpcCommissionByEnrollmentAsync`
- ✅ `ICommissionService` interface
- ✅ Hangfire jobs: monthly agent commission (1st at 2am), salary lockout (day 13 at 8pm — cron built from appsettings)
- ✅ `CommissionController`: GET by earner, GET by period, POST block (manual manager action only)
- ✅ DI registered in Program.cs

### Pending
- ⚠️ EF migration needed: `dotnet ef migrations add AddCommissions`

---

## ❌ Remaining P0 Critical Workflows

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
- [ ] **DOM-65**: Add `ApproveExpense(approverUserId)` method to Expense entity
- [ ] **DOM-66**: Add `RejectExpense(approverUserId, reason)` method to Expense entity
- [ ] **DOM-67**: Add `MarkAsPaid(paymentMethod, referenceCode, paidAt)` method to Expense entity
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

### Story 14: Group Transfer Workflow
**Priority**: P0 - Critical  
**Story Points**: 8

**Current State**: Not implemented at all

**Tasks**:
- [ ] **DOM-9**: Add `TransferGroup(Guid newGroupId, string? reason)` method to Enrollment entity
  - Validate: Current status must be Active
  - Validate: newGroupId != current GroupId
  - Add domain event `EnrollmentTransferRequestedDomainEvent`

- [ ] **DOM-10**: Schedule clash detection in application layer
  - Load all Schedule rows for new group
  - Load all Schedule rows for student's other active enrollments
  - Check for overlapping day + time slots (standard interval overlap)

- [ ] **APP-11**: Create `TransferGroupCommand` DTO
- [ ] **APP-12**: Add `TransferGroupAsync` to IEnrollmentService
- [ ] **APP-13**: Implement `TransferGroupAsync` in EnrollmentService
  - Validate same Level and Subject
  - Validate new group has available capacity (atomic)
  - Validate no schedule clashes
  - Update GroupId, adjust capacities, save with transaction, audit log

- [ ] **APP-14**: Create `TransferGroupValidator` using FluentValidation
- [ ] **API-15**: Add `POST /api/enrollments/{id}/transfer` endpoint in EnrollmentController

---

## ❌ Remaining P1 Important Workflows

### Story 9: Payment Plan Discount & Fee Evaluation
**Priority**: P1 - Important  
**Story Points**: 4

**Tasks**:
- [ ] **DOM-75**: Add `CalculateNetPayable()` method to Plan entity
- [ ] **DOM-76**: Add `GenerateInstallmentSchedule()` method to Plan entity
- [ ] **APP-77**: Update invoice generation to use dynamic due dates
- [ ] **APP-78**: Add discount override audit logging

---

### Story 10: Media Polymorphic Ownership & Storage Governance
**Priority**: P1 - Important  
**Story Points**: 3

**Tasks**:
- [ ] **DOM-93**: Add `ValidateOwner()` method to Media entity
- [ ] **APP-94**: Add storage quota validation (file extensions, MIME types, max sizes)
- [ ] **APP-95**: Integrate validation into MediaService

---


### Story 13: Automated Background Jobs & Notifications
**Priority**: P2 - Medium  
**Story Points**: 4

**Tasks**:
- [ ] **INF-100**: Create `LeadFollowUpReminderProcessor` background service
- [ ] **INF-101**: Create event-driven notification service (Email/SMS templates)
- [ ] **INF-102**: Register background services in Program.cs

---

## Summary

| Story | Priority | Status |
|---|---|---|
| Story 1: Invoice Overdue Notification | P0 | ✅ Done |
| Story 3: Group Transfer | P0 | ❌ Pending (renamed Story 14) |
| Story 5: Commission Calculation Engine | P0 | ✅ Done |
| Story 6: Commission Clawback Rules | P0 | ✅ Done (merged into Story 5) |
| Story 7: Payment Refund | P0 | ❌ Pending |
| Story 8: Expense Approval Pipeline | P0 | ❌ Pending |
| Story 9: Payment Plan Discounts | P1 | ❌ Pending |
| Story 10: Media Ownership Validation | P1 | ❌ Pending |
| Story 13: Background Jobs & Notifications | P2 | ❌ Pending |
