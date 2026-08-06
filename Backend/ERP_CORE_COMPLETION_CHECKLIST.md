# ERP Core Completion Checklist 🎯

**Goal:** Complete all missing CRUD operations and business workflows before testing  
**Status:** ~60% Complete | 40% Missing  
**Excluded:** Reports, Analytics, Email/WhatsApp handlers

---

## ✅ CRITICAL - Anti-Patterns FIXED! ✅

#### story 1. Controllers Directly Accessing DbContext (FIXED! ✅)
**All 4 controllers refactored to follow DDD architecture:**

- ✅ **GenderController** - Now uses `IGenderService` ✅
  - Created: `IGenderService`, `GenderService`, `IGenderRepository`, `GenderRepository`
  - Created: `IGenderQueryService`, `GenderQueryService`
  - Created: `GenderCommand`, `UpdateGenderCommand`, `GenderRequestDto`, `UpdateGenderRequestDto`, `GenderResponseDto`, `GenderMapper`
  - Endpoints: GET /api/genders, GET /api/genders/{id}, POST /api/genders, PUT /api/genders/{id}, DELETE /api/genders/{id}
  - **13 new files created**

- ✅ **LeadSourceController** - Now uses `ILeadSourceService` ✅
  - Created: `ILeadSourceService`, `LeadSourceService`, `ILeadSourceRepository`, `LeadSourceRepository`
  - Created: `ILeadSourceQueryService`, `LeadSourceQueryService`
  - Created: `AdLeadSourceCommand`, `OpcLeadSourceCommand`, `AdLeadSourceRequestDto`, `OpcLeadSourceRequestDto`, `LeadSourceResponseDto`, `LeadSourceMapper`
  - Endpoints: GET /api/lead-sources, GET /api/lead-sources/{id}, POST /api/lead-sources/ad, POST /api/lead-sources/opc, DELETE /api/lead-sources/{id}
  - **Polymorphism handled:** Separate endpoints for Ad and Opc types
  - **13 new files created**

- ✅ **OpcController** - Now uses `IOpcService` ✅
  - Created: `IOpcService`, `OpcService`, `IOpcRepository`, `OpcRepository`
  - Created: `IOpcQueryService`, `OpcQueryService`
  - Created: `OpcCommand`, `UpdateOpcCommand`, `OpcRequestDto`, `UpdateOpcRequestDto`, `OpcResponseDto`, `OpcMapper`
  - Endpoints: GET /api/opcs, GET /api/opcs/{id}, POST /api/opcs, PUT /api/opcs/{id}, DELETE /api/opcs/{id}
  - **13 new files created**

- ✅ **AdController** - Now uses `IAdService` ✅
  - Created: `IAdService`, `AdService`, `IAdRepository`, `AdRepository`
  - Created: `IAdQueryService`, `AdQueryService`
  - Created: `AdCommand`, `UpdateAdCommand`, `AdRequestDto`, `UpdateAdRequestDto`, `AdResponseDto`, `AdMapper`
  - Endpoints: GET /api/ads, GET /api/ads/{id}, POST /api/ads, PUT /api/ads/{id}, DELETE /api/ads/{id}
  - **13 new files created**

**✅ DI Registration:** Updated `Program.cs` with 12 new service registrations  
**✅ Total Files Created:** 52 files across all layers (Domain, Application, Infrastructure, API)  
**✅ Pattern Followed:** RequestDto → Command → Controller → Service → QueryService → Mapper → Repository → ResponseDto  
**✅ All follow DDD:** Entity factory methods (Create()), domain validation, audit logging, current user context

---

## 🟠 HIGH PRIORITY - Missing Complete Implementations

### **Academic Management (Missing 4 Entities)**

#### story 2. **Teacher** - NO Controller, NO Service, NO Repository
**Impact:** Cannot manage teachers at all!
- ❌ Controller: `TeacherController` (CRUD + Assign to Groups)
- ❌ Service: `ITeacherService`, `TeacherService`
- ❌ Repository: `ITeacherRepository`, `TeacherRepository`
- ❌ Workflows:
  - Register teacher
  - Assign to groups
  - Assign specialization/subjects
  - Update salary
  - View assigned groups

#### story 3. **Level** - NO Controller, NO Service, Repository exists
**Impact:** Cannot manage academic levels (Beginner, Intermediate, Advanced)
- ❌ Controller: `LevelController`
- ❌ Service: `ILevelService`, `LevelService`
- ✅ Repository: `ILevelRepository` exists
- Workflows needed:
  - CRUD operations
  - Reorder levels (Order field)

#### story 4. **Room** - NO Controller, NO Service, Repository exists
**Impact:** Cannot manage classrooms for scheduling
- ❌ Controller: `RoomController`
- ❌ Service: `IRoomService`, `RoomService`
- ✅ Repository: `IRoomRepository` exists
- Workflows needed:
  - CRUD operations
  - Check availability
  - View capacity

#### story 5. **Subject** - NO Controller, NO Service, Repository exists
**Impact:** Cannot manage subjects/courses
- ❌ Controller: `SubjectController`
- ❌ Service: `ISubjectService`, `SubjectService`
- ✅ Repository: `ISubjectRepository` exists
- Workflows needed:
  - CRUD operations
  - Assign to levels
  - Link to teachers

---

### **Financial Management (Missing 4 Entities)**

#### story 6. **Plan** - NO Controller, NO Service, Repository interface exists
**Impact:** Cannot manage payment plans (1 Month, 3 Months, Full Year)
- ❌ Controller: `PlanController`
- ❌ Service: `IPlanService`, `PlanService`
- ✅ Repository: `IPlanRepository` interface exists
- Workflows needed:
  - CRUD operations
  - Activate/Deactivate plans
  - Update pricing

#### story 7. **Charge** - NO Controller, Has Service partially
**Impact:** Cannot manually create/manage invoice charges
- ❌ Controller: `ChargeController`
- ⚠️ Service: Charge logic exists in `InvoiceService` but no dedicated service
- ✅ Domain: Rich entity with Waive(), Cancel(), AddPayment()
- Workflows needed:
  - View charges by invoice
  - Waive charge (partial/full)
  - Cancel charge

#### story 8. **Refund** - NO Controller, Has Service
**Impact:** Cannot view/manage refunds via API
- ❌ Controller: `RefundController`
- ✅ Service: `IRefundService` exists
- ✅ Domain: Rich entity
- Workflows needed:
  - Get refunds by payment
  - Get refunds by student
  - View refund history

#### story 9. **PayrollPayment** - NO Controller, NO Service, Repository exists
**Impact:** Cannot manage staff salary payments
- ❌ Controller: `PayrollController` or `PayrollPaymentController`
- ❌ Service: `IPayrollService`, `PayrollService`
- ✅ Repository: `IPayrollPaymentRepository` exists
- Workflows needed:
  - Create payroll payment
  - Mark as paid
  - View by employee
  - View by date range

---

### **Staff Management (Missing 3 Entities)**

#### story 10. **CommercialAgent** - NO Controller, NO Service, NO Repository
**Impact:** Cannot manage sales agents who bring students
- ❌ Controller: `CommercialAgentController`
- ❌ Service: `ICommercialAgentService`, `CommercialAgentService`
- ❌ Repository: `ICommercialAgentRepository`, `CommercialAgentRepository`
- Workflows needed:
  - Register agent
  - View assigned intakes
  - View commissions
  - Update salary



#### story 12. **DomainUser (Staff Login)** - Incomplete
**Impact:** Cannot manage staff user accounts
- ❌ Controller: `UserController` or `StaffController`
- ⚠️ Service: Has `IDomainUserRepository` but incomplete
- Workflows needed:
  - Create staff login
  - Assign roles
  - Reset password
  - Activate/Deactivate

---

### **Configuration/Setup (Missing 2 Entities)**

#### story 13. **Platform** - NO Controller, NO Service, NO Repository
**Impact:** Cannot manage social media platforms for tracking
- ❌ Controller: `PlatformController`
- ❌ Service: `IPlatformService`, `PlatformService`
- ❌ Repository: `IPlatformRepository`, `PlatformRepository`
- Workflows needed:
  - CRUD operations (Facebook, Instagram, TikTok, etc.)

#### story 14. **Branch** - NO Controller, NO Service, Repository exists
**Impact:** Cannot manage school branches via API
- ❌ Controller: `BranchController`
- ❌ Service: `IBranchService`, `BranchService`
- ✅ Repository: `IBranchRepository` exists
- Workflows needed:
  - CRUD operations
  - View branch statistics

---

### **Academic Records (Missing 2 Entities)**

#### story 15. **Absence** - NO Controller, NO Service, NO Repository
**Impact:** Cannot track student attendance
- ❌ Controller: `AbsenceController`
- ❌ Service: `IAbsenceService`, `AbsenceService`
- ❌ Repository: `IAbsenceRepository`, `AbsenceRepository`
- Workflows needed:
  - Mark student absent
  - View absence by student
  - View absence by group/date

#### story 16. **Grade** - NO Controller, NO Service, NO Repository
**Impact:** Cannot record student grades/scores
- ❌ Controller: `GradeController`
- ❌ Service: `IGradeService`, `GradeService`
- ❌ Repository: `IGradeRepository`, `GradeRepository`
- Workflows needed:
  - Record grade
  - View grades by student
  - View grades by subject

---

## 🟡 MEDIUM PRIORITY - Missing Business Workflows

#### story 18. **Student Branch Transfer** - Missing
- ❌ POST `/api/students/{id}/transfer-branch` - Transfer student to different branch
- ✅ Domain: `Student.UpdateBranchId()` exists

#### story 19. **Student Parent Management** - Service exists but no dedicated endpoints
- ✅ Service: `StudentResponsableService` exists
- ❌ GET `/api/students/{id}/parents` - View student's parents
- ❌ POST `/api/students/{id}/parents` - Add parent
- ❌ DELETE `/api/students/{id}/parents/{parentId}` - Remove parent

---

### **Enrollment Workflows (3 Missing)**

#### story 20. **Enrollment Group Transfer** - Domain exists but no endpoint
- ❌ POST `/api/enrollments/{id}/transfer-group` - Move student to different group
- ✅ Domain: `Enrollment.TransferToGroup()` exists
- ✅ Event: `EnrollmentGroupTransferredDomainEvent` exists

#### story 21. **Enrollment Drop** - Domain exists but no endpoint
- ❌ POST `/api/enrollments/{id}/drop` - Drop student from enrollment
- ✅ Domain: `Enrollment.DropEnrollment()` exists
- ✅ Event: `EnrollmentDroppedDomainEvent` exists

#### story 22. **Enrollment Complete** - Domain exists but no endpoint
- ❌ POST `/api/enrollments/{id}/complete` - Mark enrollment as completed
- ✅ Domain: `Enrollment.CompleteEnrollment()` exists
- ✅ Event: `EnrollmentCompletedDomainEvent` exists

---

### **Invoice Workflows (2 Missing)**

#### story 23. **Invoice Waive** - Domain exists but no endpoint
- ❌ POST `/api/invoices/{id}/waive` - Waive invoice (full/partial)
- ✅ Domain: `Invoice.WaiveInvoice()` exists
- ✅ Event: `InvoiceWaivedDomainEvent` exists

#### story 24. **Invoice Cancel** - Domain exists but no endpoint
- ❌ POST `/api/invoices/{id}/cancel` - Cancel invoice
- ✅ Domain: `Invoice.CancelInvoice()` exists
- ✅ Event: `InvoiceCancelledDomainEvent` exists

---

### **Financial Workflows (2 Missing)**

#### story 25. **Expense Management** - Service exists but needs workflows
- ✅ Service: `ExpenseService` exists
- ✅ Controller: `ExpenseController` exists (basic CRUD)
- ⚠️ Missing:
  - Approve expense
  - Categorize expense
  - View by date range

#### story 26. **Commission Tracking** - Entity exists but NO implementation
- ❌ All layers missing for `Commission` entity
- Workflows needed:
  - Calculate commission for agent
  - Mark commission as paid
  - View by agent

---

## 🟢 LOW PRIORITY - Nice to Have

### **Schedule Management Enhancements**

#### story 27. **Schedule Conflict Detection** - Missing
- ❌ GET `/api/schedules/conflicts` - Detect scheduling conflicts
- ❌ Room double-booking detection
- ❌ Teacher double-booking detection

#### story 28. **Group Capacity Management** - Partial
- ⚠️ Group has `MaxCapacity` but no enforcement
- ❌ GET `/api/groups/{id}/available-spots` - Check available spots
- ❌ Prevent enrollment if group is full

---

## 📊 Summary Statistics

| Category | Total Entities | Implemented | Missing | % Complete |
|----------|----------------|-------------|---------|------------|
| **Core Business** | 9 | 4 | 5 | 44% |
| **Financial** | 8 | 4 | 4 | 50% |
| **Academic** | 8 | 4 | 4 | 50% |
| **CRM** | 5 | 1 | 4 | 20% |
| **Staff** | 4 | 0 | 4 | 0% |
| **Configuration** | 4 | 0 | 4 | 0% |
| **TOTAL** | **38** | **13** | **25** | **34%** |

---

## 🎯 Recommended Implementation Order

### **Phase 1: Fix Critical Anti-Patterns (2-3 hours)**
1. Fix GenderController → Add service layer
2. Fix LeadSourceController → Add service layer
3. Fix OpcController → Add service + repository
4. Fix AdController → Add service + repository

### **Phase 2: Core Academic (4-5 hours)**
5. Implement Teacher (Controller + Service + Repository)
6. Implement Subject (Controller + Service)
7. Implement Level (Controller + Service)
8. Implement Room (Controller + Service)

### **Phase 3: Financial Completion (3-4 hours)**
9. Implement Plan (Controller + Service)
10. Implement Refund (Controller only - service exists)
11. Add Invoice workflows (Waive, Cancel endpoints)
12. Implement Charge (Controller)

### **Phase 4: Staff Management (3-4 hours)**
13. Implement CommercialAgent (Full stack)
14. Implement Employee/Staff (General staff)
15. Complete DomainUser implementation

### **Phase 5: Enrollment Workflows (2-3 hours)**
16. Add Enrollment.Drop endpoint
17. Add Enrollment.Complete endpoint
18. Add Enrollment.TransferGroup endpoint

### **Phase 6: Student Workflows (2-3 hours)**
19. Add Student credit management endpoints
20. Add Student parent management endpoints
21. Add Student branch transfer endpoint

### **Phase 7: Nice to Have (3-4 hours)**
22. Implement Absence tracking
23. Implement Grade recording
24. Implement Platform management
25. Implement Branch management
26. Implement PayrollPayment
27. Implement Commission tracking
28. Add Schedule conflict detection

---

## 🚀 Total Estimated Work

- **Phase 1-3 (Critical):** ~10-12 hours
- **Phase 4-6 (High Priority):** ~7-10 hours
- **Phase 7 (Nice to Have):** ~3-4 hours
- **TOTAL:** ~20-26 hours

---

## ✅ What's Already Complete

### **Working Controllers:**
1. ✅ StudentController - CRUD
2. ✅ StudentRegistrationController - Registration flow
3. ✅ EnrollmentController - CRUD
4. ✅ GroupController - CRUD
5. ✅ InvoiceController - CRUD + Generate
6. ✅ PaymentController - CRUD + Process
7. ✅ ExpenseController - CRUD
8. ✅ ScheduleController - CRUD + Generate
9. ✅ IntakeController - CRUD + Convert
10. ✅ MediaController - Upload/Download
11. ✅ WhatsAppController - Queue messages
12. ✅ LoginController - Authentication

### **Working Services:**
13. ✅ StudentService
14. ✅ StudentRegistrationService
15. ✅ StudentResponsableService
16. ✅ EnrollmentService
17. ✅ GroupService
18. ✅ InvoiceService
19. ✅ PaymentService
20. ✅ ExpenseService
21. ✅ RefundService
22. ✅ ScheduleService
23. ✅ IntakeService
24. ✅ WhatsAppService

---

**Ready to start? Pick a phase and let's complete it! 🚀**
