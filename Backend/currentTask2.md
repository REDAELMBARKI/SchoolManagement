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

## ✅ Story 7: Cash Refund System - COMPLETE

### Design Decisions
- **All refunds are cash in-person** — regardless of original payment method (Cash or Card)
- **Separate Refunds table** (Option B) — clarity and scalability for partial/multi-refund support
- **Partial refunds supported** — via `Payment.GetRefundableAmount()` minus sum of existing refunds
- **Manual process** — student comes to school to collect cash, recorded in system

### Refund Status Lifecycle
```
Payment created              → Payment.Status = Paid, Refunded = false
Refund recorded (partial)    → Refund created, Payment.Refunded = false
Refund recorded (full)       → Refund created, Payment.Refunded = true (auto-marked)
```

### What was built
- ✅ `Refund` entity (AggregateRoot): `Create()`, `PaymentId`, `RefundAmount`, `Reason`, `RefundedAt`, `ProcessedByUserId`
- ✅ `IRefundRepository` with custom queries
- ✅ `RefundRepository` implementation
- ✅ `RefundConfiguration` EF Core config + registered in `AppDbContext`
- ✅ `RefundService`: `RefundPaymentAsync()` with transaction rollback on failure
- ✅ `Payment.GetRefundableAmount()` — calculates remaining refundable amount
- ✅ `Payment.MarkAsRefunded()` — auto-called when fully refunded
- ✅ `Invoice.DeductRefund(amount)` — reverses PaidAmount
- ✅ `Charge.ReversePayment(amount)` — reverses PaidAmount on charge
- ✅ `PaymentController.RefundPayment` — POST /api/payments/{id}/refund endpoint
- ✅ Refund validation: payment must exist, amount > 0, amount <= refundable
- ✅ Invoice status recalculation after refund
- ✅ Credit balance reversal if payment was applied as credit
- ✅ Audit logging for refund operations
- ✅ DI registered in Program.cs

### Pending
- ⚠️ EF migration needed: `dotnet ef migrations add AddRefunds`

---

## ✅ Story 8: Expense CRUD — Cash Outflow Tracking - COMPLETE

**Architecture**: Expenses follow a simple CRUD model — **no approval pipeline**.
Cash already left the drawer; the expense record is the historical entry for financial
reporting and net-gain calculation (Net Gain = Total Payments Received − Total Expenses
Recorded). Salaries are tracked separately via the `PayrollPayment` entity.

### What was built
- ✅ `Expense` entity (AggregateRoot) — simple CRUD, no approval workflow
  - `Create()` factory method with all required fields
  - Update methods for all properties
- ✅ `ExpenseType` enum — Salary, Vendor, Utilities, Maintenance, Supplies, Rent, Other
- ✅ `PaymentMethod` enum — Cash, CreditCard, DebitCard, BankTransfer, Check
- ✅ `IExpenseRepository` interface in Domain/Core/Interfaces
- ✅ `ExpenseRepository` implementation in Infrastructure/Core/Repositories
- ✅ `IExpenseQueryService` interface with filtering (by branch, date range, category, staff)
- ✅ `ExpenseQueryService` implementation in Infrastructure/Core/Queries
- ✅ `IExpenseService` interface in Application/Core/Interfaces/Services
- ✅ `ExpenseService` implementation with full CRUD operations
- ✅ Expense DTOs: `ExpenseCommand`, `UpdateExpenseCommand`, Request/Response DTOs
- ✅ `ExpenseValidator` in Application/Core/Validators
- ✅ `ExpenseMapper` in Application/Core/Mappers
- ✅ `ExpenseController` with CRUD endpoints:
  - POST   /api/expenses
  - GET    /api/expenses/{id}
  - GET    /api/expenses (filtered: branch, date range, category, staff)
  - PUT    /api/expenses/{id}
  - DELETE /api/expenses/{id}
- ✅ `ExpenseConfiguration` EF Core config
- ✅ DI registration in Program.cs

### Pending
- ⚠️ EF migration needed: `dotnet ef migrations add AddExpenses`

---

## ❌ Remaining P0 Critical Workflows
**Priority**: P0 - Critical  
**Story Points**: 8

 **Current State**: ✅ Implemented

**Tasks**:
- [x] **DOM-9**: Add `TransferGroup(Guid newGroupId, string? reason)` method to Enrollment entity
  - Validate: Current status must be Active
  - Validate: newGroupId != current GroupId
  - Add domain event `EnrollmentGroupTransferredDomainEvent`

- [x] **DOM-10**: Schedule clash detection in application layer
  - Load all Schedule rows for new group
  - Load all Schedule rows for student's other active enrollments
  - Check for overlapping day + time slots (standard interval overlap)

- [x] **APP-11**: Create `TransferGroupCommand` DTO
- [x] **APP-12**: Add `TransferGroupAsync` to IEnrollmentService
- [x] **APP-13**: Implement `TransferGroupAsync` in EnrollmentService
  - Validate same Level and Subject
  - Validate new group has available capacity (atomic)
  - Validate no schedule clashes
  - Update GroupId, adjust capacities, save with transaction, audit log

- [x] **APP-14**: Create `TransferGroupRequestDto` for API input
- [x] **API-15**: Add `POST /api/enrollments/{id}/transfer` endpoint in EnrollmentController

**What was built**:
- ✅ `Enrollment.TransferGroup()` domain method with validation
- ✅ `EnrollmentGroupTransferredDomainEvent` event
- ✅ `TransferGroupCommand` DTO in Application layer
- ✅ `TransferGroupRequestDto` for API
- ✅ `IScheduleQueryService.GetSchedulesByGroupIdAsync()` method
- ✅ `ScheduleQueryService.GetSchedulesByGroupIdAsync()` implementation
- ✅ `EnrollmentService.TransferGroupAsync()` with:
  - Same Level/Subject validation
  - Group capacity validation
  - Schedule clash detection (checks day + time overlap using TimeSlot.StartTime/EndTime)
  - Atomic transaction with optimistic concurrency on groups
  - Audit logging
  - Domain event publishing
- ✅ `POST /api/enrollments/{id}/transfer` endpoint with full error handling
- ✅ Removed obsolete `Group.ScheduleId` property (groups now have many schedules)
- ✅ Updated `ScheduleConfiguration` to map Group.Schedules collection

**Pending**:
- ⚠️ EF migration needed for: Group.ScheduleId removal, refunds table, commissions table

---

## ❌ Remaining P1 Important Workflows

### Story 16: Refactor CreditBalance from Enrollment to Student (Domain Fix)
**Priority**: P1 - Important (Prerequisite for Story 15)  
**Story Points**: 5

**Problem**: `CreditBalance` is currently stored on the `Enrollment` entity, but this creates confusion:
- A student with 3 enrollments has 3 separate credit balances
- Credit cannot be shared across subjects (English credit can't pay for Math)
- Overpayments on one enrollment can't be applied to another
- Doesn't match real-world school accounting (credit belongs to student, not enrollment)

**Solution**: Move `CreditBalance` from `Enrollment` entity to `Student` entity.

**Current State**: ✅ **IMPLEMENTED** (Code complete, migration ready)

**Business Rule**: A student has ONE credit balance that can be used for any enrollment/invoice.

**Tasks**:
- [x] **DOM-86**: Add `CreditBalance` property to `Student` entity
  - ✅ Added `public decimal CreditBalance { get; private set; }` with default 0

- [x] **DOM-87**: Add credit management methods to `Student` entity
  - ✅ `AddCredit(decimal amount)` - increases credit balance
  - ✅ `UseCredit(decimal amount)` - decreases credit balance with validation
  - ✅ `UpdateCreditBalance(decimal amount)` - direct setter with validation

- [x] **DOM-88**: Remove `CreditBalance` from `Enrollment` entity
  - ✅ Removed property
  - ✅ Removed `AddCredit()`, `UseCredit()`, `UpdateCreditBalance()` methods
  - ✅ Removed `creditBalance` parameter from `Enrollment.Create()` factory

- [x] **INF-89**: Update `StudentConfiguration` EF Core config
  - ✅ Added `CreditBalance` column: decimal(18,2), required, default 0

- [x] **INF-90**: Update `EnrollmentConfiguration` EF Core config
  - ✅ Removed `CreditBalance` column configuration

- [x] **APP-91**: Update all services that use `Enrollment.CreditBalance`
  - ✅ Updated `PaymentService.StoreOverpaymentAsCreditAsync()` → uses `student.AddCredit()`
  - ✅ Updated `InvoiceService` renewal logic → uses `enrollment.Student.CreditBalance`
  - ✅ Added `IStudentRepository` dependencies to both services

- [x] **APP-92**: Update `PaymentService` credit logic
  - ✅ Overpayment: calls `student.AddCredit()` instead of `enrollment.AddCredit()`
  - ✅ Loads student via `enrollment.Student` navigation

- [x] **APP-93**: Update `RefundService` credit reversal logic
  - ✅ RefundService doesn't use credit directly (verified)

- [x] **APP-94**: Update DTOs and responses
  - ✅ `EnrollmentResponseDto`: removed `CreditBalance` field
  - ✅ `StudentResponseDto`: added `CreditBalance` field

- [x] **APP-95**: Update mappers
  - ✅ `EnrollmentMapper`: removed CreditBalance mapping
  - ✅ `StudentMapper`: added CreditBalance mapping

- [x] **APP-97**: Update audit snapshots
  - ✅ `EnrollmentService.CreateAuditSnapshot()`: removed CreditBalance

- [x] **MIG-98**: Create database migration script
  - ✅ Created `Migration_CreditBalance_MoveToStudent.sql`
  - ✅ Adds `Students.CreditBalance` column
  - ✅ Migrates existing enrollment credits → sum per student
  - ✅ Drops `Enrollments.CreditBalance` column
  - ✅ Includes rollback script (note: original distribution cannot be restored)

**Impact Analysis**:
- ✅ Simplifies credit logic (one balance instead of N balances)
- ✅ Enables credit sharing across enrollments
- ✅ Matches real-world accounting
- ⚠️ Breaking change: APIs that return `EnrollmentResponseDto.CreditBalance` will need frontend updates
- ⚠️ Migration required: combines existing enrollment credits into student credit

**Pending**:
- ⚠️ Migration NOT yet run: `Migrations/Migration_CreditBalance_MoveToStudent.sql` ready to execute

---

### Story 15: Enroll Existing Student in Additional Group (Multi-Subject Enrollment)
**Priority**: P1 - Important  
**Story Points**: 3

**Context**: A student who already exists and has at least one enrollment wants to enroll in another subject (e.g., they're already taking English, now they want to add Math). The student is already known/selected (e.g., from their profile page).

**Prerequisites**: Story 16 must be completed first (CreditBalance moved to Student entity).

**Current State**: Not implemented. Currently, enrollment creation flow requires full student + enrollment info. Need a simplified flow when student already exists.

**Key Difference from Regular Enrollment**:
- Student already exists → only need StudentId
- **Payment IS required** (enforces financial accountability - no unpaid enrollments)
- Exception: If student has sufficient CreditBalance, can use credit instead of new payment
- Must reuse schedule conflict validation from TransferGroup workflow
- Must reuse enrollment creation factory/logic (don't duplicate)

**Tasks**:
- [ ] **APP-80**: Create `EnrollStudentInAdditionalGroupCommand` 
  - Required: `StudentId`, `SubjectId`, `LevelId`
  - Optional:  `PreferedScheduleId`, `Notes`, `PlanId`
  - **Required: `PaymentData` (RegistrationPaymentRequestDto)** OR flag `UseCreditBalance: bool` , always use the payment amount as the main only check the Creditbalance  when the paiment amount in the requesst is  not enough ;

  - If `UseCreditBalance = true`: validate student has sufficient credit

- [ ] **APP-81**: Create `EnrollStudentInAdditionalGroupRequestDto` for API
  - Same fields as command
  - Validation: either PaymentData provided OR UseCreditBalance = true (not both)

- [ ] **APP-82**: Create `EnrollStudentInAdditionalGroupValidator` using FluentValidation
  - StudentId must not be empty
  - SubjectId must not be empty
  - LevelId must not be empty
  - Either PaymentData OR UseCreditBalance must be provided
  - If UseCreditBalance: amount must be > 0

- [ ] **APP-83**: Add `EnrollStudentInAdditionalGroupAsync()` to IEnrollmentService
  - Parameters: `Guid studentId`, `EnrollStudentInAdditionalGroupCommand command`
  - Returns: `EnrollmentResponseDto`

- [ ] **APP-84**: Implement `EnrollStudentInAdditionalGroupAsync()` in EnrollmentService
  - Load student from repository (NotFoundException if missing)
  - Validate no duplicate active enrollment in same subject
  - Load available groups for level/subject/branch
  - Evaluate and select group (reuse `EvaluateStudentGroup()`)
  - Check schedule conflicts (reuse `ValidateNoScheduleConflictsAsync()`)
  - **Handle payment:**
    - If `UseCreditBalance = true`: call `student.UseCredit(amount)` (validates sufficient balance)
    - If `PaymentData` provided: create payment record via `PaymentService`
  - Create enrollment using domain factory `Enrollment.Create()`
  - Create invoice and link to payment
  - Atomic transaction with group capacity guard
  - Audit logging
  - Publish domain events

- [ ] **API-85**: Add `POST /api/enrollments/student/{studentId}/enroll-additional` endpoint
  - Route parameter: `studentId`
  - Body: `EnrollStudentInAdditionalGroupRequestDto`
  - Calls `EnrollStudentInAdditionalGroupAsync()`
  - Returns 201 Created with `EnrollmentResponseDto`
  - Error handling:
    - 400 Bad Request: validation errors, missing payment data
    - 404 Not Found: student or group not found
    - 409 Conflict: schedule conflict, duplicate enrollment, insufficient capacity, insufficient credit balance


**What will be built**:
- Simplified enrollment flow for existing students
- Reuses schedule conflict detection from transfer workflow
- Reuses enrollment creation logic (no duplication)
- **Enforces payment** (either new payment OR credit balance)
- Uses student's unified CreditBalance (from Story 16)
- Clear separation: student already exists vs. new student registration

---

### Story 9: Payment Plan Discount & Fee Evaluation
**Priority**: ~~P1~~ **SKIPPED** - Not needed for current business model  
**Story Points**: 4

**Design Decision**: 
- ✅ Plan entity **ALREADY supports discounts** via nullable `DiscountPercent` property
- ✅ Default behavior: Plans created with `DiscountPercent = null` (fixed pricing, no discount)
- ✅ Admin flexibility: Can add/update/remove discount anytime via `UpdateDiscountPercent()` method
- ✅ Amount calculation: `Amount = BaseAmount - (BaseAmount × DiscountPercent / 100)` when discount exists
- ✅ Validation: Discount must be 0-100% when provided
- ❌ **No automatic discount logic needed** - school uses fixed pricing strategy
- ❌ **No manager override discounts** - not part of current business model
- ❌ **No promotional/seasonal discounts** - fixed pricing per plan duration

**Business Context**:
The school operates with **fixed pricing without discounts**. Plans are priced competitively from the start:
- Example: "3 Month Plan" = 3000 MAD (no discount needed, already best price)
- If market conditions change, admin can add discount later (e.g., 10% summer promotion)
- Current implementation provides flexibility without complexity

**What exists (no changes needed)**:
- ✅ `Plan.DiscountPercent` property (nullable decimal)
- ✅ `Plan.Create()` with optional `discountPercent` parameter (defaults to null)
- ✅ `Plan.UpdateDiscountPercent()` method for admin overrides
- ✅ `Plan.Amount` calculated property handles both scenarios
- ✅ Validation prevents invalid discount ranges

**Tasks** (Not implementing):
- [ ] ~~**DOM-75**: Add `CalculateNetPayable()` method to Plan entity~~ (Already exists via `Amount` property)
- [ ] ~~**DOM-76**: Add `GenerateInstallmentSchedule()` method to Plan entity~~ (Not needed - fixed pricing)
- [ ] ~~**APP-77**: Update invoice generation to use dynamic due dates~~ (Not needed)
- [ ] ~~**APP-78**: Add discount override audit logging~~ (Can add later if discount usage increases)

---

### Story 10: Media Polymorphic Ownership & Storage Governance
**Priority**: P1 - Important  
**Story Points**: 3  
**Estimated Time**: ~2.5 hours

---

#### ✅ What's Already Implemented

**1. Media Entity** ✅ Complete  
Location: `SchoolManagement.Domain/Common/Entities/Media.cs`
- ✅ Polymorphic ownership via `OwnerType` + `OwnerId`
- ✅ `OwnerType` enum: User, Student, Teacher, Administrator
- ✅ Media metadata: URL, MimeType, Size, AltText, Width, Height
- ✅ `MediaType` enum: Photo, Banner, Document, Video, Avatar
- ✅ `MediaCollection` enum: Avatar, Banner, Document, Photo, Video
- ✅ `StorageProvider` field (supports multiple storage backends)
- ✅ `BranchId` for multi-tenancy
- ✅ `Order` and `IsMain` for gallery management
- ✅ Full CRUD update methods with validation

**2. MediaService** ✅ Basic Upload  
Location: `SchoolManagement.Application/Common/Services/MediaService.cs`
- ✅ File upload to local storage (`/Uploads` folder)
- ✅ Unique filename generation (GUID-based)
- ✅ Media entity creation with metadata

**Current Issues**:
- ⚠️ Missing owner validation (doesn't check if owner exists)
- ⚠️ Missing file size validation
- ⚠️ Hardcoded `BranchId = Guid.Empty` (should use `ICurrentUserContext`)
- ⚠️ No image dimension extraction
- ⚠️ No storage quota enforcement
- ⚠️ Signature mismatch: controller doesn't pass `ownerId`/`ownerType` to service

**3. MediaController** ✅ Basic Endpoint  
Location: `SchoolManagement.Api/Controllers/MediaController.cs`
- ✅ POST `/api/media/upload` endpoint
- ✅ File extension validation: `.png`, `.jpg`, `.jpeg`
- ✅ MIME type validation: `image/jpeg`, `image/png`, `image/webp`

**Current Issues**:
- ⚠️ Hardcoded allowed extensions/MIME types (should be configurable)
- ⚠️ No max file size check
- ⚠️ Missing `ownerId` and `ownerType` parameters
- ⚠️ No owner existence validation

---

#### ❌ What Needs to Be Built

**Phase 1: Configuration & Validation** (30 minutes)
- [ ] **APP-94a**: Create `MediaStorageSettings.cs` options class in Application layer
  ```csharp
  public class MediaStorageSettings
  {
      public Dictionary<MediaType, long> MaxFileSizes { get; set; }
      public Dictionary<MediaType, string[]> AllowedExtensions { get; set; }
      public Dictionary<MediaType, string[]> AllowedMimeTypes { get; set; }
      public int BranchQuotaGB { get; set; }
  }
  ```

- [ ] **APP-94b**: Add `MediaStorage` section to `appsettings.json`
  ```json
  {
    "MediaStorage": {
      "MaxFileSizes": {
        "Photo": 5242880,      // 5 MB
        "Avatar": 2097152,      // 2 MB
        "Document": 10485760,   // 10 MB
        "Video": 104857600,     // 100 MB
        "Banner": 5242880       // 5 MB
      },
      "AllowedExtensions": {
        "Photo": [".png", ".jpg", ".jpeg", ".gif", ".webp"],
        "Avatar": [".png", ".jpg", ".jpeg"],
        "Document": [".pdf", ".doc", ".docx", ".xls", ".xlsx"],
        "Video": [".mp4", ".mov", ".avi"],
        "Banner": [".png", ".jpg", ".jpeg", ".webp"]
      },
      "AllowedMimeTypes": {
        "Photo": ["image/jpeg", "image/png", "image/gif", "image/webp"],
        "Avatar": ["image/jpeg", "image/png"],
        "Document": ["application/pdf", "application/msword", "text/plain"],
        "Video": ["video/mp4", "video/quicktime"],
        "Banner": ["image/jpeg", "image/png", "image/webp"]
      },
      "BranchQuotaGB": 10
    }
  }
  ```

- [ ] **APP-94c**: Create `MediaStorageValidator.cs` class
  - `ValidateFile(IFormFile file, MediaType mediaType)` - checks extension, MIME, size
  - `ValidateBranchQuota(Guid branchId, long newFileSize)` - checks total storage used

- [ ] **APP-94d**: Register in DI (Program.cs)
  ```csharp
  builder.Services.Configure<MediaStorageSettings>(builder.Configuration.GetSection("MediaStorage"));
  builder.Services.AddScoped<MediaStorageValidator>();
  ```

**Phase 2: Owner Validation** (20 minutes)
- [ ] **DOM-93 (Alternative)**: Add `ValidateOwnerExists()` private method to MediaService (Application layer)
  - Validates Student exists via `IStudentRepository.GetByIdAsync()`
  - Validates User exists via `IUserRepository.GetByIdAsync()`
  - Validates Teacher exists via `ITeacherRepository.GetByIdAsync()`
  - Throws `NotFoundException` if owner doesn't exist
  - **Design Decision**: Keep validation in Application layer (not Domain) to avoid repository coupling

**Phase 3: Service Enhancement** (40 minutes)
- [ ] **APP-95a**: Update `IMediaService.Upload()` signature
  ```csharp
  Task<MediaResponseDto> Upload(
      IFormFile file, 
      Guid ownerId, 
      OwnerType ownerType, 
      MediaCollection collection, 
      MediaType mediaType);
  ```

- [ ] **APP-95b**: Inject dependencies into MediaService
  - Add `IStudentRepository`, `IUserRepository`, `ITeacherRepository`
  - Add `MediaStorageValidator`
  - Add `ICurrentUserContext`
  - Add `IAuditLogService`

- [ ] **APP-95c**: Update `MediaService.Upload()` implementation
  - Call `_validator.ValidateFile(file, mediaType)` first
  - Call `_validator.ValidateBranchQuota(branchId, file.Length)` second
  - Call `ValidateOwnerExists(ownerId, ownerType)` third
  - Fix `branchId` to use `_currentUserContext.BranchId` (not Guid.Empty)
  - Add audit logging after successful upload
  - *(Optional)* Extract image dimensions using ImageSharp

**Phase 4: Controller Update** (15 minutes)
- [ ] **API-95d**: Update `MediaController.Upload()` signature
  ```csharp
  [HttpPost]
  public async Task<IActionResult> Upload(
      IFormFile file,
      [FromForm] Guid ownerId,
      [FromForm] OwnerType ownerType,
      [FromForm] MediaCollection collection,
      [FromForm] MediaType mediaType)
  ```

- [ ] **API-95e**: Remove hardcoded validation from controller (moved to service layer)
  - Remove extension checks
  - Remove MIME type checks
  - Keep null/empty file check

- [ ] **API-95f**: Add proper error handling
  - Catch `ValidationException` → return 400 Bad Request
  - Catch `NotFoundException` → return 404 Not Found
  - Catch generic exceptions → return 500 Internal Server Error

**Phase 5: Repository Extension** (Optional, 20 minutes)
- [ ] **INF-95g**: Add `GetTotalSizeByBranchAsync(Guid branchId)` to IMediaRepository
  ```csharp
  Task<long> GetTotalSizeByBranchAsync(Guid branchId);
  ```

- [ ] **INF-95h**: Implement in MediaRepository
  ```csharp
  public async Task<long> GetTotalSizeByBranchAsync(Guid branchId)
  {
      return await _context.Media
          .Where(m => m.BranchId == branchId)
          .SumAsync(m => m.Size);
  }
  ```

---

#### 🎯 Success Criteria

✅ **Owner Validation**:
- Uploading media with non-existent owner returns 404
- Owner type validated correctly (Student, User, Teacher, Administrator)

✅ **File Validation**:
- Invalid extensions rejected per MediaType
- Invalid MIME types rejected per MediaType
- Files exceeding size limit rejected with clear error message
- Configuration-driven (admin can adjust limits in appsettings.json)

✅ **Storage Governance**:
- Branch quota enforced (optional, configurable)
- Total storage tracked per branch

✅ **Audit Trail**:
- All uploads logged with owner info, file size, MIME type

---

#### 🔧 Technical Decisions

**Decision 1: Owner Validation Location**  
✅ **Chosen**: Application layer (MediaService)  
**Rationale**: Domain layer should not depend on repositories (architectural purity)

**Decision 2: Storage Configuration**  
✅ **Chosen**: appsettings.json  
**Rationale**: Easy to update, environment-specific, no admin UI needed (YAGNI)

**Decision 3: Image Dimension Extraction**  
⏭️ **Recommended**: Use ImageSharp library (cross-platform, modern)  
📦 **Package**: `SixLabors.ImageSharp` (optional enhancement)

---

#### 📦 Optional NuGet Package

```bash
# For image dimension extraction (optional but recommended)
dotnet add package SixLabors.ImageSharp
```

---

#### 📋 Implementation Checklist Summary

| Phase | Tasks | Time |
|---|---|---|
| Configuration & Validation | Create settings, validator | 30 min |
| Owner Validation | Add validation logic | 20 min |
| Service Enhancement | Update MediaService | 40 min |
| Controller Update | Update MediaController | 15 min |
| Repository Extension | Add quota query (optional) | 20 min |
| Testing | Full validation testing | 30 min |
| **TOTAL** | | **~2.5 hours** |

---

**Tasks**:
- [ ] **APP-94a-d**: Configuration & validation setup
- [ ] **DOM-93**: Owner validation (in Application layer)
- [ ] **APP-95a-c**: Service enhancement
- [ ] **API-95d-f**: Controller update
- [ ] **INF-95g-h**: Repository extension (optional)

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
| Story 5: Commission Calculation Engine | P0 | ✅ Done |
| Story 6: Commission Clawback Rules | P0 | ✅ Done (merged into Story 5) |
| Story 7: Payment Refund | P0 | ✅ Done |
| Story 8: Expense CRUD (Cash Outflow Tracking) | P0 | ✅ Done |
| Story 14: Group Transfer | P0 | ✅ Done |
| Story 16: Refactor CreditBalance to Student | P1 | ✅ Done (migration ready) |
| Story 15: Enroll Existing Student in Additional Group | P1 | ✅ Done (ready for testing) |
| Story 9: Payment Plan Discounts | ~~P1~~ SKIPPED | ✅ Not needed (fixed pricing) |
| Story 10: Media Ownership Validation | P1 | ❌ Pending |
| Story 13: Background Jobs & Notifications | P2 | ❌ Pending |
