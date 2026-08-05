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
**Status**: ✅ **COMPLETED** (Registration + Service extracted)

**What's Been Built** ✅:
- ✅ `StudentResponsableRequestDto` - DTO for parent/guardian info
- ✅ `StudentResponsableResponseDto` - Response DTO
- ✅ `StudentResponsableValidator` - FluentValidation for parent data
- ✅ `IStudentResponsableRepository` + implementation - Repository pattern
- ✅ `IStudentResponsableService` + `StudentResponsableService` - Service layer with slug generation, linking, audit logging
- ✅ Parent creation during student registration - Optional `ResponsableRegReq` in `StudentRegistrationRequestDto`
- ✅ Automatic linking - Parent linked to student during registration via many-to-many relationship
- ✅ Audit logging for parent creation
- ✅ StudentRegistrationService refactored to pure orchestration (delegates to StudentResponsableService)

**What's Pending** ⚠️:
- [ ] **API-165**: Separate endpoints for parent management (add/remove after registration)
  - POST `/api/students/{studentId}/responsables` - Add additional parent/guardian
  - GET `/api/students/{studentId}/responsables` - List all responsables for a student
  - DELETE `/api/students/{studentId}/responsables/{responsableId}` - Unlink parent from student
  - PUT `/api/students/responsables/{responsableId}` - Update parent/guardian info

**Business Rules** ✅:
- ✅ Student can have multiple parents/guardians (many-to-many relationship)
- ✅ Parent info is optional during registration
- ✅ `RelationshipType` enum: Father, Mother, Guardian, Grandfather, Grandmother, Uncle, Aunt, Other

**Guardian Portal Permissions** (Future - out of scope for now):
- [ ] Permission aggregator for parent access
- [ ] Read-only access to student data (transcripts, invoices)

---

## 🟡 MEDIUM PRIORITY - Resource & Staff Management

### 3. Branch CRUD
**Priority**: P2 - Admin Management  
**Story Points**: 2  
**Status**: ❌ Not implemented

**What Needs to Be Built**:
- [ ] **APP-200**: Create `IBranchService` interface + `BranchService`
  - `GetAllAsync()` - List all branches
  - `GetByIdAsync(Guid id)` - Get branch by ID
  - `CreateAsync(BranchCommand)` - Create new branch with slug generation
  - `UpdateAsync(Guid id, UpdateBranchCommand)` - Update branch details
  - `DeleteAsync(Guid id)` - Soft delete branch
  
- [ ] **API-201**: Create `BranchController`
  - GET `/api/branches` - List all branches
  - GET `/api/branches/{id}` - Get single branch
  - POST `/api/branches` - Create branch
  - PUT `/api/branches/{id}` - Update branch
  - DELETE `/api/branches/{id}` - Delete branch

**Domain Already Complete** ✅:
- ✅ `Branch.Create()` factory method
- ✅ Update methods: `UpdateName`, `UpdateSlug`, `UpdateCity`, `UpdateAddress`, `UpdatePhone`

---

### 4. Room CRUD
**Priority**: P2 - Resource Management  
**Story Points**: 2  
**Status**: ❌ Not implemented

**What Needs to Be Built**:
- [ ] **APP-210**: Create `IRoomService` interface + `RoomService`
  - `GetAllAsync(Guid? branchId)` - List all rooms (optionally filtered by branch)
  - `GetByIdAsync(Guid id)` - Get room by ID
  - `CreateAsync(RoomCommand)` - Create new room
  - `UpdateAsync(Guid id, UpdateRoomCommand)` - Update room details
  - `DeleteAsync(Guid id)` - Soft delete room
  
- [ ] **API-211**: Create `RoomController`
  - GET `/api/rooms?branchId={id}` - List all rooms
  - GET `/api/rooms/{id}` - Get single room
  - POST `/api/rooms` - Create room
  - PUT `/api/rooms/{id}` - Update room
  - DELETE `/api/rooms/{id}` - Delete room

**Domain Already Complete** ✅:
- ✅ `Room.Create()` factory method
- ✅ Update methods: `UpdateName`, `UpdateCapacity`, `UpdateFloor`, `UpdateDescription`, `UpdateBranchId`

---

### 5. Level CRUD
**Priority**: P2 - Academic Configuration  
**Story Points**: 2  
**Status**: ❌ Not implemented

**What Needs to Be Built**:
- [ ] **APP-220**: Create `ILevelService` interface + `LevelService`
  - `GetAllAsync(Guid? branchId)` - List all levels (optionally filtered by branch)
  - `GetByIdAsync(Guid id)` - Get level by ID
  - `CreateAsync(LevelCommand)` - Create new level
  - `UpdateAsync(Guid id, UpdateLevelCommand)` - Update level details
  - `DeleteAsync(Guid id)` - Soft delete level
  
- [ ] **API-221**: Create `LevelController`
  - GET `/api/levels?branchId={id}` - List all levels
  - GET `/api/levels/{id}` - Get single level
  - POST `/api/levels` - Create level
  - PUT `/api/levels/{id}` - Update level
  - DELETE `/api/levels/{id}` - Delete level

**Domain Already Complete** ✅:
- ✅ `Level.Create()` factory method
- ✅ Update methods: `UpdateName`, `UpdateBranchId`, `UpdateOrder`

---

### 6. Platform CRUD
**Priority**: P2 - Marketing Configuration  
**Story Points**: 2  
**Status**: ❌ Not implemented

**What Needs to Be Built**:
- [ ] **APP-230**: Create `IPlatformService` interface + `PlatformService`
  - `GetAllAsync(Guid? branchId)` - List all platforms (optionally filtered by branch)
  - `GetByIdAsync(Guid id)` - Get platform by ID
  - `CreateAsync(PlatformCommand)` - Create new platform with slug generation
  - `UpdateAsync(Guid id, UpdatePlatformCommand)` - Update platform details
  - `DeleteAsync(Guid id)` - Soft delete platform
  
- [ ] **API-231**: Create `PlatformController`
  - GET `/api/platforms?branchId={id}` - List all platforms
  - GET `/api/platforms/{id}` - Get single platform
  - POST `/api/platforms` - Create platform
  - PUT `/api/platforms/{id}` - Update platform
  - DELETE `/api/platforms/{id}` - Delete platform

**Domain Already Complete** ✅:
- ✅ `Platform.Create()` factory method
- ✅ Update methods: `UpdateName`, `UpdateSlug`, `UpdateBranchId`

---

### 7. Teacher CRUD
**Priority**: P2 - Staff Management  
**Story Points**: 3  
**Status**: ❌ Not implemented

**What Needs to Be Built**:
- [ ] **APP-240**: Create `ITeacherService` interface + `TeacherService`
  - `GetAllAsync(Guid? branchId)` - List all teachers (optionally filtered by branch)
  - `GetByIdAsync(Guid id)` - Get teacher by ID
  - `CreateAsync(TeacherCommand)` - Create new teacher with slug generation
  - `UpdateAsync(Guid id, UpdateTeacherCommand)` - Update teacher details (name, email, phone, salary, specialization)
  - `DeleteAsync(Guid id)` - Soft delete teacher
  - `AssignSubjectAsync(Guid teacherId, Guid subjectId)` - Link teacher to subject (TeacherSubject table)
  - `RemoveSubjectAsync(Guid teacherId, Guid subjectId)` - Unlink teacher from subject
  
- [ ] **API-241**: Create `TeacherController`
  - GET `/api/teachers?branchId={id}` - List all teachers
  - GET `/api/teachers/{id}` - Get single teacher
  - POST `/api/teachers` - Create teacher
  - PUT `/api/teachers/{id}` - Update teacher
  - DELETE `/api/teachers/{id}` - Delete teacher
  - POST `/api/teachers/{id}/subjects` - Assign subject to teacher
  - DELETE `/api/teachers/{id}/subjects/{subjectId}` - Remove subject from teacher

**Domain Already Complete** ✅:
- ✅ `Teacher.Register()` factory method
- ✅ Update methods: `UpdateEmail`, `UpdatePhone`, `UpdateSalary`, `UpdateSpecialization`
- ✅ `TeacherSubject` join entity for many-to-many relationship

---

### 8. Commercial Agent CRUD
**Priority**: P2 - Staff Management  
**Story Points**: 3  
**Status**: ❌ Not implemented

**What Needs to Be Built**:
- [ ] **APP-250**: Create `ICommercialAgentService` interface + `CommercialAgentService`
  - `GetAllAsync(Guid? branchId)` - List all agents (optionally filtered by branch)
  - `GetByIdAsync(Guid id)` - Get agent by ID
  - `CreateAsync(CommercialAgentCommand)` - Create new agent with slug generation
  - `UpdateAsync(Guid id, UpdateCommercialAgentCommand)` - Update agent details (name, email, phone, salary)
  - `DeleteAsync(Guid id)` - Soft delete agent
  
- [ ] **API-251**: Create `CommercialAgentController`
  - GET `/api/agents?branchId={id}` - List all agents
  - GET `/api/agents/{id}` - Get single agent
  - POST `/api/agents` - Create agent
  - PUT `/api/agents/{id}` - Update agent
  - DELETE `/api/agents/{id}` - Delete agent

**Domain Already Complete** ✅:
- ✅ `CommercialAgent.Register()` factory method
- ✅ Inherited update methods from `Employee`: `UpdateEmail`, `UpdatePhone`, `UpdateSalary`

---

### 9. Subject CRUD
**Priority**: P2 - Academic Configuration  
**Story Points**: 2  
**Status**: ❌ Not implemented

**What Needs to Be Built**:
- [ ] **APP-260**: Create `ISubjectService` interface + `SubjectService`
  - `GetAllAsync(Guid? branchId)` - List all subjects (optionally filtered by branch)
  - `GetByIdAsync(Guid id)` - Get subject by ID
  - `CreateAsync(SubjectCommand)` - Create new subject with slug generation
  - `UpdateAsync(Guid id, UpdateSubjectCommand)` - Update subject details
  - `DeleteAsync(Guid id)` - Soft delete subject
  
- [ ] **API-261**: Create `SubjectController`
  - GET `/api/subjects?branchId={id}` - List all subjects
  - GET `/api/subjects/{id}` - Get single subject
  - POST `/api/subjects` - Create subject
  - PUT `/api/subjects/{id}` - Update subject
  - DELETE `/api/subjects/{id}` - Delete subject

**Note**: Subject entity exists but needs to be checked for domain methods

---

### 10. Plan CRUD
**Priority**: P2 - Financial Configuration  
**Story Points**: 2  
**Status**: ❌ Not implemented

**What Needs to Be Built**:
- [ ] **APP-270**: Create `IPlanService` interface + `PlanService`
  - `GetAllAsync()` - List all plans
  - `GetByIdAsync(Guid id)` - Get plan by ID
  - `CreateAsync(PlanCommand)` - Create new plan with slug generation
  - `UpdateAsync(Guid id, UpdatePlanCommand)` - Update plan details (price, duration, discounts)
  - `DeleteAsync(Guid id)` - Soft delete plan
  
- [ ] **API-271**: Create `PlanController`
  - GET `/api/plans` - List all plans
  - GET `/api/plans/{id}` - Get single plan
  - POST `/api/plans` - Create plan
  - PUT `/api/plans/{id}` - Update plan
  - DELETE `/api/plans/{id}` - Delete plan

**Note**: Plan entity exists but needs to be checked for domain methods

---

## 🟢 LOW PRIORITY - Reporting & Analytics (Excluded from MVP)

### 11. Financial Reporting & Aging Analysis
**Priority**: P3 - Reporting  
**Story Points**: 6  
**Status**: ❌ Not implemented (excluded from current scope)

**Decision**: Defer to post-MVP phase. Focus on core transactional features first.

---

### 12. Operational Analytics & Lead Funnel  
**Priority**: P3 - Analytics  
**Story Points**: 5  
**Status**: ❌ Not implemented (excluded from current scope)

**Decision**: Defer to post-MVP phase. Focus on core transactional features first.

---

### 13. Automated Background Jobs & Notifications
**Priority**: P3 - Infrastructure  
**Story Points**: 6  
**Status**: ⚠️ **Partially implemented** (invoice overdue job done, others deferred)

**What's Done**:
- ✅ OverdueInvoiceProcessor (daily job)
- ✅ Commission calculation jobs (monthly OPC, agent, salary lockout)

**What's Deferred** (Post-MVP):
- [ ] Lead follow-up reminder notifications
- [ ] Email/SMS notification service
- [ ] Absence alert system
- [ ] Invoice issued notifications
- [ ] Payment receipt notifications

**Decision**: Core financial operations work without these. Defer to post-MVP phase.

---

## 🔵 EXCLUDED - Out of Scope for ERP MVP

### ❌ Attendance Management (Academic - Excluded)
**Reason**: Academic feature - not core ERP functionality

### ❌ Grade & Transcript Engine (Academic - Excluded)
**Reason**: Academic feature - not core ERP functionality

### ❌ Academic Analytics (Academic - Excluded)
**Reason**: Academic feature - not core ERP functionality



## 📊 Summary Dashboard (ERP Features Only)

| Feature | Priority | Story Points | Status |
|---------|----------|--------------|--------|
| ~~**Intake Lead Conversion**~~ | ~~P0~~ | ~~0~~ | ✅ **Done (Frontend-Assisted)** |
| **Schedule CRUD & Conflict Detection** | P1 | 6 | ✅ **COMPLETED** |
| ~~**Teacher Qualification**~~ | ~~P1~~ | ~~4~~ | ❌ **REMOVED** |
| **Parent-Student Linking** | P1 | 5 | ✅ **COMPLETED** |
| **Branch CRUD** | P2 | 2 | ❌ Not started |
| **Room CRUD** | P2 | 2 | ❌ Not started |
| **Level CRUD** | P2 | 2 | ❌ Not started |
| **Platform CRUD** | P2 | 2 | ❌ Not started |
| **Teacher CRUD** | P2 | 3 | ❌ Not started |
| **Commercial Agent CRUD** | P2 | 3 | ❌ Not started |
| **Subject CRUD** | P2 | 2 | ❌ Not started |
| **Plan CRUD** | P2 | 2 | ❌ Not started |
| **Financial Reporting** | P3 | 6 | ❌ Deferred (Post-MVP) |
| **Operational Analytics** | P3 | 5 | ❌ Deferred (Post-MVP) |
| **Background Jobs & Notifications** | P3 | 6 | ⚠️ Partial (40% done, rest deferred) |

**Total Remaining Story Points**: ~18 story points (P2 features only)

**Completed**:
- ✅ **Intake Lead Conversion** (0 pts) - Frontend-assisted via existing `/api/students/register` endpoint
- ✅ **Schedule CRUD & Conflict Detection** (6 pts) - 13 methods + 6 endpoints + 2 validators + AJAX support
- ✅ **Parent-Student Linking** (5 pts) - Service layer + registration integration complete

**Excluded (Academic - Out of Scope)**:
- ❌ Attendance Management (6 pts)
- ❌ Grade & Transcript Engine (7 pts)
- ❌ Academic Analytics (attendance rates, academic performance)

**Deferred to Post-MVP**:
- ⏸️ Financial Reporting (6 pts)
- ⏸️ Operational Analytics (5 pts)
- ⏸️ Notification system completion (3 pts remaining)

**Estimated Timeline** (assuming 1 developer, 8 story points/week):
- ~2.5 weeks for all remaining P2 features (Resource & Staff Management CRUD)

---

## 🎯 Recommended Next Steps (ERP Focus Only)

### Immediate (This Week):
1. ✅ **Parent-Student Service Refactoring** - COMPLETED
2. **Build & test StudentResponsable endpoints** - Complete CRUD for parent management
   - POST `/api/students/{studentId}/responsables` - Add parent after registration
   - GET `/api/students/{studentId}/responsables` - List student's parents
   - PUT `/api/students/responsables/{responsableId}` - Update parent info
   - DELETE `/api/students/{studentId}/responsables/{responsableId}` - Unlink parent

### Next Sprint (Resource Management - Priority Order):
1. **Branch CRUD** (P2, 2 pts) - Multi-branch support foundation
2. **Room CRUD** (P2, 2 pts) - Required for schedule conflict detection
3. **Level CRUD** (P2, 2 pts) - Student progression management
4. **Platform CRUD** (P2, 2 pts) - Marketing channel tracking

### Following Sprint (Staff Management):
5. **Teacher CRUD** (P2, 3 pts) - Staff management + subject assignment
6. **Commercial Agent CRUD** (P2, 3 pts) - Sales team management
7. **Subject CRUD** (P2, 2 pts) - Academic offering configuration
8. **Plan CRUD** (P2, 2 pts) - Pricing model management

### Later (Post-MVP - Reporting & Analytics):
9. **Financial Reporting** (P3, 6 pts) - Revenue breakdown, aging reports, profitability
10. **Operational Analytics** (P3, 5 pts) - Group occupancy, lead funnel, retention metrics
11. **Complete Notifications** (P3, 3 pts remaining) - Email/SMS for invoices, payments, lead reminders

---

**Document Version**: 2.0  
**Last Updated**: August 1, 2026  
**Changes**:
- ✅ Marked Parent-Student Linking as COMPLETED (service extracted, registration integration done)
- Added 8 CRUD features for resource & staff management (18 story points)
- Moved reporting/analytics to P3 (deferred to post-MVP)
- Excluded academic features (attendance, grades, academic analytics)

**Next Review**: After completing P2 CRUD features
