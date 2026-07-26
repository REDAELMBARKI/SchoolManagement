# Next Tasks — Non-CRUD Business Workflows

Priority: P0 (must have) / P1 (important) / P2 (nice to have)

---

## P0 — Critical / Blocking Workflows

### 1. Charge Life Cycle — Payment Application + Auto Status (Paid/PartiallyPaid/Overdue)
- **Problem now**: `ChargeService.CreateAsync` / `UpdateAsync` are thin. Registering a new standalone Payment (outside `StudentRegistrationService`) never calls `Charge.AddPayment()` to roll up `AmountPaid` or flip status. There's no overdue detection.
- **Logic needed**:
  - On any `Payment.Create` that links to a `ChargeId`: load the Charge, `charge.AddPayment(payment.Amount)`, persist, so `AmountPaid` and `Status` stay correct.
  - Mark charge `Overdue` when `DueDate` passes and status is still `Unpaid` / `PartiallyPaid` (either via query service filter or on-demand job / filter).
  - Waiving / cancelling a charge: use `Cancelled` / `Waived` statuses (see project_memory — not zero-amount payments).
- **Scope**: `PaymentService.Create/Update`, new `ChargeService.ApplyPaymentAsync`, maybe a `ChargeService.WaiveAsync / CancelAsync`. Validator: Payment amount must not exceed the charge remaining balance (unless explicitly allowed with overpayment rule).

### 2. Enrollment Drop / Complete + Group Capacity Release
- **Problem now**: `EnrollmentService.Update` only changes fields (no status change logic, no side effects). Groups have `Capacity` but enrollment count never decrements/increments on status change.
- **Logic needed**:
  - `DropEnrollment(enrollmentId, reason)`: set Status = Dropped. If paid, optionally auto-create a refund charge (or flag). Release the group seat: recalc group enrolled count.
  - `CompleteEnrollment(enrollmentId)`: set Status = Completed, but keep the seat since it's historical; optionally auto-flag related charges as non-collectible if balance remains.
  - Prevent updates once Dropped/Completed (like Intake blocks updates when HasStudents).
  - New enrollment → ensure group `EnrolledCount < Capacity` on save (current check only via `AvailableGroupsByLevelSubjectBranch` which relies on query count mismatch: make it a real atomic/DB-checked rule).

### 3. Intake → Student Conversion (Status Lifecycle + AmountPaid Guard)
- **Problem now**: Intake has `HasStudents` guard and Student can link to Intake, but there's no explicit "convert intake to student" workflow. `UpdateIntake` blocks when HasStudents, but conversion itself isn't automated.
- **Logic needed**:
  - `ConvertIntakeToStudent(intakeId, enrollment command?)`: Create Student, auto-set Intake Status = `Enrolled`; if Intake.AmountPaid > 0 carry that over.
  - Intake status guard: only allow conversion from statuses `Interested` / `Contacted` / `New` (whatever the business says — but disallow `NotInterested` or `Enrolled` already).
  - Prevent double-conversion: one Intake can produce many Students (since list exists), but each Intake+status=Enrolled should require at least 1 student, or a "ConvertedAt" flag.

### 4. Commercial Agent Commission from Intake Conversion
- **Problem now**: Intake has `CommercialAgentId`, `TotalFees`, `AmountPaid` — but no commission engine. No agent performance tracker.
- **Logic needed**:
  - When an Intake converts to Student (or when enrollment payment is received): compute commission = agreed % × `TotalFees` (or × AmountPaid, TBD by business rule).
  - New `Commission` aggregate (or add field to Expense as "Agent Commission" expense type).
  - Clawback rule if enrollment drops within N days (reversal of commission expense).
  - Reporting: per-agent monthly conversions, collected amount, commission earned.

### 5. Absence Recording → Attendance Tracking + Justification + Student Reach
- **Problem now**: `Absence` entity exists; no AbsenceService/Controller/validator.
- **Logic needed**:
  - `RecordAbsence(scheduleId, studentId, type: Absent|Late|Present, minutesLate, reason?, justified?)`.
  - Guardrails:
    - Student must be enrolled in the Group linked to that Schedule.
    - No duplicate Absence for same (Student, Schedule, Date).
    - Marking absent triggers a domain event → handler can (a) auto-set follow-up task for teacher/commercial agent, (b) auto-count towards N-absences rule (expulsion/warning threshold on Student).
  - `JustifyAbsence(absenceId, reason)` flips `IsJustified = true` and updates reason.

### 6. Grades Workflow — Issue/Revise + Transcript/Average Rollup
- **Problem now**: `Grade` entity exists; no service/controller/validator.
- **Logic needed**:
  - `IssueGrade(groupTeacherId, studentId, gradeType: Exam|Quiz|HW, weight, score, maxScore, issuedAt?)`: guard that Student is in that GroupTeacher's group; no duplicate grade for same assignment.
  - `ReviseGrade(gradeId, newScore, reason)`: immutable versioning or a RevisionLog.
  - For a Student + Subject + Level: auto-calculate Weighted Average = Σ(score/maxScore × weight) / Σ weight, grouped by term/semester.
  - Transcript query service with term averages and overall GPA.

### 7. Payment Refund / Reversal
- **Problem now**: `PaymentService.Delete` just deletes (hard delete). No refund concept; no Charge/Enrollment side effects.
- **Logic needed**:
  - `RefundPayment(paymentId, amount?, reason, method)`:
    - Payment status transitions (new statuses? Or negative amount via a sibling Refund entity — cleaner).
    - If payment was applied to a charge: reverse `Charge.AmountPaid` by the refunded amount → status flips back to PartiallyPaid/Unpaid.
    - Optionally re-open enrollment-related things.
    - All refund ops must be auditable (AuditLog).

### 8. Expense Approval Workflow
- **Problem now**: `Expense` entity has `RequestedByUserId`, `ApprovedByUserId`, `Status` (Pending/Approved/Rejected/Paid/etc). No Expense service/controller.
- **Logic needed**:
  - `SubmitExpense(expense)` → auto `Status = Pending`, fill `RequestedByUserId` from current user.
  - `ApproveExpense(expenseId, approverUserId)` → guard: approver ≠ requester, approver has a role permission (e.g. Admin/Finance). Flip status to Approved.
  - `RejectExpense(expenseId, reason)` → flip status.
  - `MarkPaid(expenseId, paymentMethod, paymentRef, paidAt)`: when the cash actually leaves the account.
  - Report: monthly expenses by category, payee, branch.

---

## P1 — Important, Not Blocking

### 9. Plan Discount Rules + Remaining-Amount Auto Due Date Logic
- **Logic needed**:
  - Plan.DiscountPercent validation: 0–100.
  - For an enrollment: `NetPayable = Plan.BaseAmount × (1 − Plan.DiscountPercent/100)`. Currently the code in `EvaluatePaymentPlanAsync` uses `plan.Amount` but Plan entity has `BaseAmount` and `DiscountPercent` — they should produce one NetPayable.
  - `RemainingAmountDueDate = EnrollmentDate.AddDays(Plan.RemainingAmountDueDays)` — currently hardcoded against `DateTime.UtcNow`; should be derived.
  - Tiered discounts: possibly `DiscountPercent` overridable per-enrollment (manager override) with audit log.

### 10. Schedule Clash Detection (Teacher / Room / Student)
- **Logic needed**:
  - On Schedule create/update:
    - Teacher cannot have two overlapping Schedules same day.
    - Room cannot have two overlapping Schedules same day.
    - A Student cannot be enrolled in two Groups whose Schedules clash (checked at Enrollment time or reported as a warning).
  - Time overlap rule: `(A.Start < B.End) AND (B.Start < A.End)` within same Day.

### 11. Teacher Assignments (GroupTeacher + TeacherSubject) + Capacity
- **Logic needed**:
  - Assign teachers to group sections (GroupTeacher entity exists — no service/validator).
  - A teacher cannot be assigned to more than N groups/week (configurable teacher max-hours capacity).
  - A teacher must already be linked to the subject via TeacherSubject before assigning them to a Group for that subject.
  - Unassign → check no future Schedule entries, or reschedule.

### 12. Student-Parent Linking + Parent Portal Access Prep
- **Logic needed**:
  - `LinkParentToStudent(parentId, studentId, relationship)`: currently Student has Parents collection — add a relationship label (Father/Mother/Guardian/…).
  - A parent can't be linked twice to the same student.
  - A parent may have a `DomainUser` / `ApplicationUser` link for future portal login.
  - On parent account creation → auto grant view-only to all linked Students' invoices/attendance/grades.

### 13. Media Owner Validation + Referential Integrity
- **Logic needed**:
  - `Media.Create(ownerType, ownerId, ...)`: validate that ownerId actually exists (Student/Teacher/DomainUser/Branch) — currently no foreign key enforcement (it's polymorphic via OwnerType enum).
  - Soft-delete owner → soft-delete/hide their media.
  - File size / dimension limits enforcement (field exists but no validation).

### 14. Audit Log Population
- **Problem now**: `AuditLog` entity exists. No middleware/saver that actually writes rows.
- **Logic needed**:
  - Hook into EF Core `SaveChangesAsync` override in `AppDbContext` (or a repo decorator). For every `Added / Modified / Deleted` entity that should be audited: serialize old/new values, set EntityType, EntityId, UserId, BranchId, Action.
  - Exclude identity/security tables and large blobs (Media content).

---

## P2 — Nice to Have

### 15. Reporting: Financial Statements
- Revenue by Branch / Plan / Month.
- Charges aging (0–30, 31–60, 61–90, 90+ days overdue).
- Expenses vs Revenue per Branch.

### 16. Reporting: Operational
- Group fill rate: enrolled / capacity per group.
- Teacher utilization: taught-hours / max-capacity-hours.
- Intake conversion funnel per LeadSource (Ad vs OPC): New → Contacted → Interested → Enrolled counts + %.
- Student attendance rate per Subject, per Group, per Month.

### 17. Background Jobs / Notifications
- Daily: mark charges Overdue, emit domain event.
- Daily: alert CommercialAgents about "Intakes with FollowUpDate = today".
- Email/SMS (placeholder): on Absence → parent notification; on Grade issued → student/parent notification; on Payment received → receipt.

### 18. Branch Context + Multi-Tenancy Hardening
- Ensure every read/write explicitly filters by `BranchId` — not just services that currently call `_currentUserContext.BranchId`. Make a test/analyzer that catches repository calls without BranchId.
- Cross-branch admin role: allow super-admin to switch branch context.

### 19. Roles / Claims / Policies (ASP.NET Identity + DomainUser)
- Currently `ApplicationUser` uses Identity roles but no explicit policy attributes on controllers.
- Define policies: IntakeWriter, FinanceApprover, TeacherOnly, ParentReadonly, BranchAdmin.
- Wire `ICurrentUserContext` with roles from Identity claims.

---

## Pure CRUD Endpoints Still Missing (controllers/services — low priority, scaffoldable)
These are entities with no API surface yet; add once their workflows above land:
- Branch, Charge (partial), Expense, Grade, Absence, Level, Platform, CommercialAgent, Parent, Plan, Payment (partial), Teacher, TeacherSubject, GroupTeacher, Room, Day, TimeSlot, User/DomainUser management
