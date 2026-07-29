# Next Tasks — Non-CRUD Business Workflows

**Focus**: Pure business logic, domain rules, state transitions, financial workflows, and multi-tenant domain services. *(Simple CRUD scaffoldings are excluded or listed separately at the bottom as low priority.)*

Priority Legend: **P0 (Critical / Blocking)** | **P1 (Important Domain Rules)** | **P2 (Reporting & Analytics)**

---

## 🟢 Authenticated Status & System Architecture Alignment
- **Audit Logging Integration (`IAuditLogService`)**: Fully implemented in `SchoolManagement.Infrastructure` and integrated into domain application services (`StudentService`, `PaymentService`, `EnrollmentService`, `InvoiceService`, `ChargeService`, `GroupService`, `IntakeService`).
- **Invoice Billing Model**: System migrated from direct Charges to `Invoice` Aggregate (`Invoice` containing line-item `Charge`s and handling payments via `Invoice.AddPayment()`).

---

## P0 — Critical Business Workflows & State Machines

### 1. Invoice Lifecycle & Settlement Engine
- **Current State**: `Invoice` status transitions (`Pending`, `Paid`, `PartiallyPaid`, `Overdue`) currently trigger only during manual payment creation/updates. There is no automated overdue tracking background job or formal waiver/cancellation workflow.
- **Business Logic Needed**:
  - **Automated Overdue Engine**: A background worker or query filter that scans active `Invoice` records past `DueDate` with `PaidAmount < TotalAmount` and transitions status to `InvoiceStatus.Overdue`.
  - **Invoice Waiver & Cancellation Workflow**: `CancelInvoice(invoiceId, reason)` and `WaiveInvoice(invoiceId, waivedAmount, reason)`. Waived amounts must reduce the net balance without generating dummy payment entries.
  - **Overpayment & Credit Balance Handling**: When a payment exceeds invoice total, automatically route remaining funds into `Enrollment.CreditBalance` or flag an unapplied credit balance for future invoices.

### 2. Enrollment Lifecycle, Group Capacity & Group Transfers
- **Current State**: `EnrollmentService.UpdateAsync` performs plain property updates without domain state machine logic. Group capacity checks rely on soft query counts without concurrency guards or atomic seat count management.
- **Business Logic Needed**:
  - **Drop Enrollment Workflow**: `DropEnrollment(enrollmentId, reason)`. Transition status to `Dropped`. Release group capacity seat (`EnrolledCount`). Trigger invoice cancellation/waiver or retain liability based on drop policy.
  - **Complete Enrollment Workflow**: `CompleteEnrollment(enrollmentId)`. Transition status to `Completed`. Retain seat history while locking further fee modifications.
  - **Group Transfer Workflow**: `TransferGroup(enrollmentId, newGroupId)`. Validate that `newGroupId` belongs to the same level/subject, has available capacity (`EnrolledCount < Capacity`), and has no schedule clashes with the student's existing active enrollments. Decrement old group seat count and increment new group seat count.
  - **Atomic Capacity Guard**: Enforce concurrency-safe capacity validation when creating or moving enrollments.

### 3. Intake Lead Conversion & Commercial Agent Commission Engine
- **Current State**: `Intake` records store `CommercialAgentId`, `OpcId`, `TotalFees`, `AmountPaid`, but lack an automated conversion pipeline and commission engine.
- **Business Logic Needed**:
  - **Lead Conversion Pipeline**: `ConvertIntakeToStudent(intakeId, registrationDetails)`: Automatically create `Student`, create `Enrollment`, generate initial `Invoice`, transition `Intake.Status` to `Converted`, and flag `Intake.HasStudents = true`.
  - **Commission Calculation Engine**: On enrollment payment or conversion, calculate agent commission (`Commission = Agreed% × TotalFees` or `AmountPaid`). Record transactions in a `Commission` ledger.
  - **Clawback Rule**: If an enrollment is dropped within `N` days (e.g., 14-day trial period), trigger a commission clawback/reversal event.
  - **Agent Performance Metrics**: Track conversion funnel metrics (% converted, lead source efficiency, average deal size, earned commission).

### 4. Absence Recording & Attendance Management Workflow
- **Current State**: `Absence` entity exists in Domain (`StudentId`, `ScheduleId`, `Date`, `Status`), but lacks domain services, controllers, and validation rules.
- **Business Logic Needed**:
  - **Record Class Attendance**: `RecordAttendance(scheduleId, date, attendanceRecords: [{studentId, status: Present|Absent|Late, minutesLate, reason}])`.
  - **Domain Guardrails**: Verify student is actively enrolled in the group bound to that schedule entry. Prevent duplicate attendance records for the same student/schedule/date combination.
  - **Absence Threshold Alerts**: Track cumulative unexcused absences per student per subject. Emit domain events (`StudentAbsenceThresholdExceeded`) when a student reaches `N` unexcused absences (triggering agent/parent alerts).
  - **Absence Justification**: `JustifyAbsence(absenceId, reason, documentationRef)`. Flips status to justified and adjusts cumulative threshold count.

### 5. Grade & Transcript Calculation Engine
- **Current State**: `Grade` entity exists in Domain (`StudentId`, `SubjectId`, `Score`, `MaxScore`, `Weight`), but lacks evaluation and calculation logic.
- **Business Logic Needed**:
  - **Issue Grade**: `IssueGrade(groupTeacherId, studentId, gradeType: Exam|Quiz|Homework, weight, score, maxScore, issuedAt)`. Validate teacher is assigned to the group section and student is actively enrolled.
  - **Grade Revision & Audit Log**: `ReviseGrade(gradeId, newScore, reason)`. Create an immutable grade revision log.
  - **Weighted Average & GPA Calculation Engine**: Compute weighted score average per subject per term: `Average = Σ(Score / MaxScore × Weight) / Σ Weight`.
  - **Transcript Generator**: Aggregate subject averages across terms into academic transcripts with class rank and overall performance indicators.

### 6. Payment Reversal, Refund & Financial Reconciliation
- **Current State**: `PaymentService.DeleteAsync` performs hard deletion of payment records without reversing financial impacts on invoices or credits.
- **Business Logic Needed**:
  - **Refund / Reversal Workflow**: `RefundPayment(paymentId, refundAmount, reason, method)`.
  - **Invoice Balance Re-adjustment**: Deduct `refundAmount` from `Invoice.PaidAmount` and recalculate `Invoice.Status` (flipping back to `PartiallyPaid` or `Pending`).
  - **Credit Balance Reversal**: If refunded payment was applied as credit, reverse `Enrollment.CreditBalance`.
  - **Financial Audit Log**: Record immutable payment reversal events for accounting reconciliation.

### 7. Multi-Level Expense Approval Pipeline
- **Current State**: `Expense` entity exists in Domain (`Amount`, `Status`, `RequestedByUserId`, `ApprovedByUserId`, `ExpenseType`), but lacks service logic.
- **Business Logic Needed**:
  - **Expense Submission**: `SubmitExpense(command)` → set `Status = Pending` and populate `RequestedByUserId`.
  - **Approval / Rejection Rules**: `ApproveExpense(expenseId, approverUserId)`. Guard: approver cannot be requester (`RequestedByUserId != ApprovedByUserId`), and approver must possess `FinanceApprover` role.
  - **Disbursement Tracking**: `MarkExpensePaid(expenseId, paymentMethod, referenceCode, paidAt)`. Flip status to `Paid` and record cash outflow.
  - **Financial Reporting**: Aggregate branch monthly expenses vs. revenue.

---

## P1 — Important Domain Rules & Workflows

### 8. Payment Plan Discount & Fee Evaluation Engine
- **Business Logic Needed**:
  - **Net Payable Calculation**: `NetPayable = BaseAmount × (1 − DiscountPercent / 100)`. Apply custom manager discount overrides with recorded justification and audit log.
  - **Dynamic Due Date Scheduling**: Derive charge/invoice due dates using `Plan.RemainingAmountDueDays` relative to enrollment date rather than static dates.
  - **Installment Schedule Generator**: Generate scheduled installment charges based on plan frequency (Monthly, Term, Annual).

### 9. Schedule Clash & Resource Conflict Detection
- **Business Logic Needed**:
  - **Room Conflict Check**: Prevent scheduling two groups in the same `RoomId` at overlapping times `(StartA < EndB AND StartB < EndA)`.
  - **Teacher Conflict Check**: Prevent assigning a teacher to multiple schedules occurring at the same time.
  - **Student Conflict Warning**: Detect when a student enrolls in two groups with overlapping schedule times.

### 10. Teacher Qualification & Workload Management
- **Business Logic Needed**:
  - **Subject Qualification Check**: Ensure a teacher is linked via `TeacherSubject` before assigning them to a group section for that subject.
  - **Max Weekly Hours / Workload Capacity**: Prevent assigning a teacher to groups exceeding their maximum weekly teaching capacity.
  - **Teacher Reassignment & Rescheduling**: Workflow to reassign group sections or reschedule classes with notification triggers.

### 11. Parent-Student Linking & Guardian Portal Permissions
- **Business Logic Needed**:
  - **Relationship Link**: `LinkParentToStudent(parentId, studentId, relationship: Father|Mother|Guardian, isPrimaryContact)`.
  - **Permission Aggregator**: Grant linked parents view-only access to academic transcripts, attendance records, and pending invoices for their linked students.
  - **Duplicate Link Guard**: Prevent duplicate active parent links to the same student.

### 12. Media Polymorphic Ownership & Storage Governance
- **Business Logic Needed**:
  - **Polymorphic Owner Validation**: When creating/updating `Media(OwnerType, OwnerId)`, validate that `OwnerId` exists in the corresponding aggregate repository (`Student`, `Teacher`, `Branch`, `DomainUser`).
  - **Storage Quota & File Type Enforcement**: Validate file extensions, MIME types, and maximum byte sizes per owner category.

---

## P2 — Reporting Engines, Analytics & Background Services

### 13. Financial Reporting & Aging Analysis Engine
- Revenue breakdown by Branch, Payment Plan, and Subject.
- Charge & Invoice aging reports (0–30 days, 31–60 days, 61–90 days, 90+ days overdue).
- Branch Profitability: Revenue minus Expenses per branch per month.

### 14. Operational Analytics & Lead Funnel
- Group Occupancy Rate: `EnrolledCount / Capacity` per subject/branch.
- Commercial Lead Conversion Funnel per Lead Source (Ad vs OPC): New → Contacted → Interested → Converted.
- Student Retention & Attendance Rate per Group and Subject.

### 15. Automated Background Jobs & Event Handlers
- Daily Overdue Invoice Processor background task.
- Automated Lead Follow-up Reminder notifications for Commercial Agents.
- Event-driven Email/SMS notifications (Absence alerts, Invoice issued, Payment receipt, Grade published).

---

## Non-Workflow CRUD Endpoints (Low Priority / Pure Scaffolding)
*Simple CRUD APIs with no complex business rules (scaffold as needed):*
- Branch, Level, Platform, Gender, LeadSource, Opc, Room, Day, TimeSlot management controllers.
