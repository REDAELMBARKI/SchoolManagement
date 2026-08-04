# Current Tasks 3 - Remaining Unimplemented Features (ERP Focus Only)

**Created**: August 1, 2026  
**Focus**: Pure ERP features - Financial, Operational, Scheduling, Resource Management  
**Excluded**: Academic features (Grades, Transcripts, Academic Analytics) - Out of scope

---

## ✅ What's Been Completed (DO NOT DUPLICATE)

### From currentTask2.md:
- ✅ **Story 1**: Invoice Overdue Notification System
- ✅ **Story 5 & 6**: Commission System (OPC + Agent)
- ✅ **Story 7**: Cash Refund System
- ✅ **Story 8**: Expense CRUD
- ✅ **Story 14**: Group Transfer Workflow
- ✅ **Story 15**: Enroll Existing Student in Additional Group
- ✅ **Story 16**: CreditBalance moved to Student entity
- ✅ **Invoice Lifecycle**: Waive/Cancel methods
- ✅ **Enrollment Lifecycle**: Drop/Complete methods
- ✅ **Group Capacity**: Atomic capacity guard with optimistic concurrency
- ✅ **Schedule Conflict Detection**: ValidateNoScheduleConflictsAsync for transfers
- ✅ **Story 10**: Media Storage Governance (IN PROGRESS - needs namespace fixes)

### Design Decisions Already Made:
- ✅ **Story 9 SKIPPED**: No automatic discounts, fixed pricing model
- ✅ **Commission Clawback**: Implemented via EnrollmentDroppedCommissionHandler
- ✅ **Overpayment Handling**: Automatically stores as student credit balance
- ✅ **Refunds**: All cash in-person, partial refunds supported
- ✅ **Currency**: Removed entirely (MAD-only market)

---

## ✅ ALREADY IMPLEMENTED (DO NOT BUILD)

### ~~1. Intake Lead Conversion & Registration Pipeline~~
**Priority**: ~~P0~~ **✅ COMPLETED via Frontend-Assisted Conversion**  
**Story Points**: 0 (no backend work needed)  
**Status**: ✅ **Already Implemented**

**⚠️ IMPORTANT - DO NOT IMPLEMENT THIS TASK ⚠️**

**Design Decision**: **Frontend-Assisted Conversion** (not backend auto-conversion)

**Why Frontend-Assisted?**
- Student requires fields that Intake allows as nullable (`Phone`, `DateOfBirth`)
- User needs to review/verify data before creating student record
- Flexibility to add parent/guardian info (StudentResponsable) during registration
- Reuses existing `/api/students/register` endpoint (no new backend code needed)

**Current Implementation Flow**:
```
1. User views Intake record → clicks "Convert to Student" button
2. Frontend navigates to Student Registration form
3. Form auto-fills with Intake data:
   - FirstName, LastName, Slug, GenderId
   - Email, Phone (if available), DateOfBirth (if available)
   - BranchId, SubjectId
   - IntakeId (hidden field - for linkage)
4. User reviews/completes required fields (Phone, DateOfBirth if null)
5. User submits → Frontend calls existing POST /api/students/register
6. Backend creates: Student + Enrollment + Invoice + Payment
7. Backend automatically:
   - Links Student.IntakeId to original Intake
   - Sets Intake.HasStudents = true (via navigation)
   - Updates Intake.Status to Converted (frontend can call PUT /api/intakes/{id})
   - Triggers commission via EnrollmentCreatedDomainEvent
```

**What's Already Built** ✅:
- ✅ `POST /api/students/register` endpoint (StudentController)
- ✅ `StudentRegistrationRequestDto` with all required fields
- ✅ `Student.Register()` domain factory with IntakeId linkage
- ✅ `Enrollment.Create()` with payment handling
- ✅ Invoice generation and payment processing
- ✅ Commission trigger via domain events
- ✅ Audit logging for all operations

**Why NOT Backend Auto-Conversion?**
- ❌ Intake.Phone is nullable, Student.Phone is required
- ❌ Intake.DateOfBirth is nullable, Student.DateOfBirth is required
- ❌ No user review step (data quality risk)
- ❌ Cannot add parent/guardian info during conversion
- ❌ Less flexible (all-or-nothing approach)

**What Frontend Does**:
1. Pre-fills registration form with Intake data
2. Passes `IntakeId` when calling `/api/students/register`
3. Optionally calls `PUT /api/intakes/{id}` to update status to `Converted`

**Conclusion**: ✅ **NO BACKEND WORK NEEDED - Task Complete**

---

## 🔴 HIGH PRIORITY - Core ERP Workflows

### ~~2. Teacher Qualification & Workload Management~~
**Status**: ❌ **REMOVED - Not needed for small school context**

**Reason**: In a small school, HR manages teacher qualifications during hiring. The system doesn't need to validate if a teacher is qualified for a subject - that's already verified by HR before assigning them. The TeacherSubject table exists for reference, but no automated validation is needed.

**Decision**: Trust HR decisions. If a teacher is assigned to teach a subject, they're qualified.

---

### 1. Schedule CRUD & Conflict Detection
**Priority**: P1 - Important  
**Story Points**: 6  
**Status**: ✅ **COMPLETED**

**Design**: Frontend sends startTime/endTime, backend finds TimeSlot by match. Time overlap detection for conflicts. AJAX + backend validation.

**Implementation Complete**:

**A. Service Methods (IScheduleService)** - 6 public methods ✅
- ✅ `CreateSchedulesAsync(CreateSchedulesCommand)` - bulk create with two-phase validation
- ✅ `GetGroupScheduleAsync(Guid groupId)` - grouped by days with nested DTOs
- ✅ `UpdateScheduleAsync(Guid id, UpdateScheduleCommand)` - with conflict validation + excludeId
- ✅ `DeleteScheduleAsync(Guid id)` - soft delete via DeletedAt shadow property
- ✅ `CheckRoomAvailabilityAsync(roomId, dayId, startTime, endTime, excludeId?)` - AJAX with conflict details
- ✅ `CheckTeacherAvailabilityAsync(teacherId, dayId, startTime, endTime, excludeId?)` - AJAX with conflict details

**B. Query Methods (IScheduleQueryService)** - 3 methods ✅
- ✅ `GetSchedulesByGroupIdAsync(Guid groupId)` - with TimeSlot, Day, Room, Teacher, Subject includes
- ✅ `GetRoomSchedulesAsync(Guid roomId, Guid dayId)` - with TimeSlot, Group.Subject includes
- ✅ `GetTeacherSchedulesAsync(Guid teacherId, Guid dayId)` - with TimeSlot, Group.Subject includes

**C. Private Helpers (ScheduleService)** - 4 methods ✅
- ✅ `FindTimeSlotByTimesAsync(startTime, endTime)` - lookup with error if not found
- ✅ `ValidateNoRoomConflictAsync(roomId, dayId, startTime, endTime, excludeId?)` - throws InvalidOperationException on conflict
- ✅ `ValidateNoTeacherConflictAsync(teacherId, dayId, startTime, endTime, excludeId?)` - throws InvalidOperationException on conflict
- ✅ `HasTimeOverlap(start1, end1, start2, end2)` - time overlap: `(start1 < end2) AND (end1 > start2)`

**D. API Endpoints (ScheduleController)** - 6 endpoints ✅
- ✅ POST `/api/schedules` → CreateSchedulesAsync (bulk creation)
- ✅ GET `/api/schedules/group/{groupId}` → GetGroupScheduleAsync (day-grouped view)
- ✅ PUT `/api/schedules/{scheduleId}` → UpdateScheduleAsync (single update)
- ✅ DELETE `/api/schedules/{scheduleId}` → DeleteScheduleAsync (soft delete)
- ✅ GET `/api/schedules/check-room-availability` → CheckRoomAvailabilityAsync (AJAX validation)
- ✅ GET `/api/schedules/check-teacher-availability` → CheckTeacherAvailabilityAsync (AJAX validation)

**E. FluentValidation Validators** - 2 validators ✅
- ✅ `CreateSchedulesCommandValidator` - validates bulk creation (GUIDs exist, StartTime < EndTime)
- ✅ `UpdateScheduleCommandValidator` - validates single update (GUIDs exist, StartTime < EndTime)

**F. Response DTOs** - 5 DTOs ✅
- ✅ `GroupScheduleResponseDto` with nested `DayScheduleDto` and `SessionDto`
- ✅ `RoomAvailabilityDto` with Available flag and conflict details
- ✅ `TeacherAvailabilityDto` with Available flag and conflict details
- ✅ Supporting: `RoomInfoDto`, `TeacherInfoDto`, `SubjectInfoDto`, `ConflictDetailDto`

**Total**: ✅ 13 methods + 6 endpoints + 2 validators + 5 DTOs = **FULLY IMPLEMENTED**

**Files Created/Modified (12 files)**:
1. `SchoolManagement.Application/Academic/Dtos/Commands/CreateSchedulesCommand.cs`
2. `SchoolManagement.Application/Academic/Dtos/Commands/UpdateScheduleCommand.cs`
3. `SchoolManagement.Application/Academic/Dtos/Responses/GroupScheduleResponseDto.cs`
4. `SchoolManagement.Application/Academic/Dtos/Responses/RoomAvailabilityDto.cs`
5. `SchoolManagement.Application/Academic/Dtos/Responses/TeacherAvailabilityDto.cs`
6. `SchoolManagement.Application/Academic/Interfaces/Queries/IScheduleQueryService.cs`
7. `SchoolManagement.Application/Academic/Interfaces/Services/IScheduleService.cs`
8. `SchoolManagement.Application/Academic/Services/ScheduleService.cs`
9. `SchoolManagement.Application/Academic/Validators/CreateSchedulesCommandValidator.cs`
10. `SchoolManagement.Application/Academic/Validators/UpdateScheduleCommandValidator.cs`
11. `SchoolManagement.Infrastructure/Academic/Queries/ScheduleQueryService.cs`
12. `SchoolManagement.Api/Controllers/ScheduleController.cs`

---

### 2. Parent-Student Linking & Guardian Portal
**Priority**: P1 - Important  
**Story Points**: 5  
**Status**: ✅ **PARTIALLY COMPLETED** (Registration linking done, separate management endpoints pending)

**What's Been Built** ✅:
- ✅ `StudentResponsableRequestDto` - DTO for parent/guardian info
- ✅ `StudentResponsableValidator` - FluentValidation for parent data
- ✅ `IStudentResponsableRepository` + implementation - Repository pattern
- ✅ Parent creation during student registration - Optional `ResponsableRegReq` in `StudentRegistrationRequestDto`
- ✅ Automatic linking - Parent linked to student during registration via many-to-many relationship
- ✅ Audit logging for parent creation

**What's Pending** ⚠️:
- [ ] **API-165**: Separate endpoints for parent management (add/remove after registration)
  - POST `/api/students/{studentId}/responsables` - Add additional parent/guardian
  - GET `/api/students/{studentId}/responsables` - List all responsables for a student
  - DELETE `/api/students/{studentId}/responsables/{responsableId}` - Unlink parent from student

**Business Rules** ✅:
- ✅ Student can have multiple parents/guardians (many-to-many relationship)
- ✅ Parent info is optional during registration
- ✅ `RelationshipType` enum: Father, Mother, Guardian, Grandfather, Grandmother, Uncle, Aunt, Other

**Guardian Portal Permissions** (Future - out of scope for now):
- [ ] Permission aggregator for parent access
- [ ] Read-only access to student data (transcripts, invoices)

---

## 🟡 MEDIUM PRIORITY - Reporting & Analytics

### 3. Financial Reporting & Aging Analysis
**Priority**: P2 - Reporting  
**Story Points**: 6  
**Status**: ❌ Not implemented

**What Needs to Be Built**:

- [ ] **APP-170**: Create `IFinancialReportService` interface
  ```csharp
  Task<RevenueBreakdownDto> GetRevenueBreakdownAsync(Guid? branchId, DateTime startDate, DateTime endDate);
  Task<AgingReportDto> GetInvoiceAgingReportAsync(Guid? branchId);
  Task<ProfitabilityReportDto> GetBranchProfitabilityAsync(Guid branchId, DateTime startDate, DateTime endDate);
  ```

- [ ] **APP-171**: Implement reporting queries
  - Revenue by branch, plan, subject
  - Invoice aging buckets (0-30, 31-60, 61-90, 90+ days)
  - Branch profitability (revenue - expenses)

- [ ] **API-172**: Create `ReportsController` with endpoints
  - GET `/api/reports/revenue` - Revenue breakdown
  - GET `/api/reports/aging` - Invoice aging report
  - GET `/api/reports/profitability` - Branch profitability

---

### 4. Operational Analytics & Lead Funnel
**Priority**: P2 - Analytics  
**Story Points**: 5  
**Status**: ❌ Not implemented

**What Needs to Be Built**:

- [ ] **APP-180**: Create `IAnalyticsService` interface
  ```csharp
  Task<GroupOccupancyDto> GetGroupOccupancyReportAsync(Guid? branchId, Guid? subjectId);
  Task<LeadFunnelDto> GetLeadConversionFunnelAsync(Guid? leadSourceId, DateTime startDate, DateTime endDate);
  Task<RetentionReportDto> GetStudentRetentionReportAsync(Guid? branchId);
  ```

- [ ] **APP-181**: Implement analytics queries
    - Group occupancy rate: `EnrolledCount / Capacity`
  - Lead conversion funnel by source
  - Student retention rate (active enrollments vs total students)

- [ ] **API-182**: Create `AnalyticsController` with endpoints
  - GET `/api/analytics/occupancy` - Group occupancy
  - GET `/api/analytics/funnel` - Lead funnel
  - GET `/api/analytics/retention` - Retention report

---

### 5. Automated Background Jobs & Notifications
**Priority**: P2 - Infrastructure  
**Story Points**: 6  
**Status**: ⚠️ **Partially implemented** (invoice overdue job done, others missing)

**What's Done**:
- ✅ OverdueInvoiceProcessor (daily job)
- ✅ Commission calculation jobs (monthly OPC, agent, salary lockout)

**What's Missing**:

- [ ] **INF-190**: Create `LeadFollowUpReminderProcessor` background service
  - Daily job to check intakes with `FollowUpDate <= Today`
  - Emit `LeadFollowUpDueEvent` for agent notifications

- [ ] **INF-191**: Create `INotificationService` interface
  ```csharp
  Task SendEmailAsync(string to, string subject, string body);
  Task SendSMSAsync(string phone, string message);
  Task SendAbsenceAlertAsync(Guid studentId, int absenceCount);
  Task SendInvoiceIssuedNotificationAsync(Guid invoiceId);
  Task SendPaymentReceiptAsync(Guid paymentId);
  Task SendGradePublishedNotificationAsync(Guid gradeId);
  ```

- [ ] **INF-192**: Implement notification service with templates
  - Email/SMS templates for each notification type
  - Template variables (student name, amount, date, etc.)
  - Integration with email provider (SendGrid, SMTP)
  - Integration with SMS provider (Twilio, etc.)

- [ ] **INF-193**: Create event handlers for notifications
  - `InvoiceIssuedEventHandler` → send email/SMS
  - `PaymentCompletedEventHandler` → send receipt
  - `LeadFollowUpDueEventHandler` → send reminder to agent
  - `EnrollmentCreatedEventHandler` → send welcome message
  - `EnrollmentDroppedEventHandler` → send notification

- [ ] **INF-194**: Register background services in Program.cs
  ```csharp
  builder.Services.AddHostedService<LeadFollowUpReminderProcessor>();
  ```

---

## 🔵 LOW PRIORITY - Simple CRUD (Scaffold as needed)

These are basic CRUD endpoints with no complex business logic:

- [ ] Branch CRUD (if not already done)
- [ ] Level CRUD (if not already done)
- [ ] Platform CRUD
- [ ] Room CRUD
- [ ] Day CRUD (likely already seeded)
- [ ] TimeSlot CRUD

**Note**: Only implement these when actually needed by frontend or for admin management.



## 📊 Summary Dashboard (ERP Features Only)

| Feature | Priority | Story Points | Status |
|---------|----------|--------------|--------|
| ~~**Intake Lead Conversion**~~ | ~~P0~~ | ~~0~~ | ✅ **Done (Frontend-Assisted)** |
| **Schedule CRUD & Conflict Detection** | P1 | 6 | ✅ **COMPLETED** |
| ~~**Teacher Qualification**~~ | ~~P1~~ | ~~4~~ | ❌ **REMOVED** |
| **Parent-Student Linking** | P1 | 5 | ❌ Not started |
| **Financial Reporting** | P2 | 6 | ❌ Not started |
| **Operational Analytics** | P2 | 4 | ❌ Not started |
| **Background Jobs & Notifications** | P2 | 6 | ⚠️ Partial (40% done) |
| **Simple CRUD Endpoints** | P3 | 2-3 each | ❌ As needed |

**Total Remaining Story Points**: ~21 story points (ERP features only)

**Completed**:
- ✅ **Intake Lead Conversion** (0 pts) - Frontend-assisted via existing `/api/students/register` endpoint
- ✅ **Schedule CRUD & Conflict Detection** (6 pts) - 13 methods + 6 endpoints + 2 validators + AJAX support

**Excluded (Academic - Out of Scope)**:
- ❌ Attendance Management (6 pts)
- ❌ Grade & Transcript Engine (7 pts)
- ❌ Academic Analytics (attendance rates, academic performance)

**Estimated Timeline** (assuming 1 developer, 8 story points/week):
- ~2.5 weeks for all remaining ERP features
- 1 week for P1 features (Parent Linking only)
- 1.5 weeks for P2 features (Reporting, Analytics, Notifications)

---

## 🎯 Recommended Next Steps (ERP Focus Only)

### Immediate (This Week):
1. ✅ **Fix build errors** - resolve namespace conflicts
2. ✅ **Complete Story 10** - finish Media service implementation
3. ✅ **Run pending migrations** - CreditBalance, Currency, Commissions, Refunds, Expenses
4. ✅ **Test Story 15** - test additional enrollment endpoint
5. ✅ **Schedule CRUD & Conflict Detection** - COMPLETED (6 pts)

### Next Sprint (Priority Order):
1. **Parent-Student Linking** (P1, 5 pts) - Contact management and guardian portal prep
2. **Build & test Schedule endpoints** - Verify all 6 endpoints work correctly

### Following Sprint (Reporting & Analytics):
4. **Financial Reporting** (P2, 6 pts) - Revenue breakdown, aging reports, profitability
5. **Operational Analytics** (P2, 4 pts) - Group occupancy, lead funnel, retention metrics
6. **Complete Notifications** (P2, 6 pts) - Email/SMS for invoices, payments, lead reminders

### Later (Infrastructure & Polish):
7. **Simple CRUD** (P3, as needed) - Branch, Platform, Room management endpoints

---

**Document Version**: 1.0  
**Last Updated**: August 1, 2026  
**Next Review**: After completing P0 features
