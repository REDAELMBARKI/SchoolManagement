# Current Task 4 - Complex Business Logic (Beyond CRUD)

**Created**: August 1, 2026  
**Focus**: Complex workflows with business rules, validation, and domain events  
**Excluded**: Simple CRUD operations (deferred), Reporting/Analytics (Post-MVP)

---

## 🎯 What's This Document For?

This tracks **complex business logic features** that involve:
- Multi-step workflows with transactions
- Domain validation and business rules
- Cross-entity operations
- Background processing
- File handling with governance

**NOT included here**: Simple CRUD operations (Branch, Room, Level, Platform, Teacher, Agent, Subject, Plan) - those are deferred to a separate task.

---

## ✅ Already Completed (Don't Rebuild)

### From currentTask2.md:
- ✅ Invoice Overdue Notification System
- ✅ Commission System (OPC + Agent + Clawback)
- ✅ Cash Refund System
- ✅ Expense CRUD
- ✅ Group Transfer Workflow
- ✅ Enroll Existing Student in Additional Group
- ✅ CreditBalance Refactor (moved to Student entity)

### From currentTask3.md:
- ✅ Schedule CRUD & Conflict Detection
- ✅ Parent-Student Linking & StudentResponsableService

---

## 🔴 HIGH PRIORITY - Complex Workflows

### 1. Parent/Guardian Management Endpoints
**Priority**: P1 - Important  
**Story Points**: 3  
**Status**: ⚠️ Partial (service done, endpoints missing)

**Context**: StudentResponsableService is complete, but we need REST endpoints for post-registration parent management.

**What's Already Built** ✅:
- ✅ `StudentResponsableService.CreateAndLinkToStudentAsync()` - used during registration
- ✅ StudentResponsable domain entity with validation
- ✅ Repository layer complete

**What Needs to Be Built**:

- [ ] **APP-165a**: Add methods to IStudentResponsableService
  ```csharp
  Task<StudentResponsableResponseDto> CreateAndLinkToStudentAsync(Guid studentId, StudentResponsableRequestDto request);
  Task<List<StudentResponsableResponseDto>> GetStudentResponsablesAsync(Guid studentId);
  Task<StudentResponsableResponseDto> UpdateResponsableAsync(Guid responsableId, StudentResponsableRequestDto request);
  Task UnlinkResponsableFromStudentAsync(Guid studentId, Guid responsableId);
  ```

- [ ] **APP-165b**: Implement methods in StudentResponsableService
  - `GetStudentResponsablesAsync()` - load all parents/guardians for a student
  - `UpdateResponsableAsync()` - update parent info (name, phone, email, relationship)
  - `UnlinkResponsableFromStudentAsync()` - remove many-to-many link (doesn't delete parent entity)

- [ ] **API-165c**: Create `StudentResponsableController`
  - POST `/api/students/{studentId}/responsables` - Add parent after registration
  - GET `/api/students/{studentId}/responsables` - List all parents for student
  - PUT `/api/students/responsables/{responsableId}` - Update parent info
  - DELETE `/api/students/{studentId}/responsables/{responsableId}` - Unlink parent

**Business Rules**:
- ✅ Student can have multiple parents/guardians (many-to-many)
- ✅ Parent can be linked to multiple students (siblings)
- Unlinking removes relationship, not the parent entity (other students may still reference it)
- Update affects all students linked to that parent

**Estimated Time**: ~2 hours

---

### 2. Media Polymorphic Ownership & Storage Governance
**Priority**: P1 - Important  
**Story Points**: 3  
**Status**: ⚠️ Partial (basic upload works, governance missing)

**Context**: Media entity exists, basic upload works, but missing owner validation, file size limits, and storage governance.

**Current Issues**:
- ⚠️ No owner validation (can upload with non-existent ownerId)
- ⚠️ No file size limits
- ⚠️ Hardcoded allowed extensions/MIME types
- ⚠️ Missing branch quota enforcement
- ⚠️ Hardcoded `BranchId = Guid.Empty`

**What Needs to Be Built**:

**Phase 1: Configuration (30 min)**
- [ ] **APP-170a**: Create `MediaStorageSettings` options class
  - `MaxFileSizes` per MediaType (Photo: 5MB, Document: 10MB, Video: 100MB)
  - `AllowedExtensions` per MediaType
  - `AllowedMimeTypes` per MediaType
  - `BranchQuotaGB` (storage limit per branch)

- [ ] **APP-170b**: Add `MediaStorage` section to appsettings.json
  ```json
  {
    "MediaStorage": {
      "MaxFileSizes": {
        "Photo": 5242880,
        "Avatar": 2097152,
        "Document": 10485760
      },
      "AllowedExtensions": {
        "Photo": [".png", ".jpg", ".jpeg", ".webp"],
        "Document": [".pdf", ".doc", ".docx"]
      },
      "BranchQuotaGB": 10
    }
  }
  ```

- [ ] **APP-170c**: Create `MediaStorageValidator` class
  - `ValidateFile(IFormFile file, MediaType mediaType)` - checks size, extension, MIME
  - `ValidateBranchQuota(Guid branchId, long fileSize)` - checks total storage

**Phase 2: Owner Validation (20 min)**
- [ ] **APP-171**: Add `ValidateOwnerExists()` to MediaService
  - Check Student/User/Teacher exists via repositories
  - Throw `NotFoundException` if owner doesn't exist

**Phase 3: Service Enhancement (40 min)**
- [ ] **APP-172a**: Update `IMediaService.Upload()` signature
  - Add `ownerId`, `ownerType`, `collection`, `mediaType` parameters

- [ ] **APP-172b**: Inject dependencies into MediaService
  - `IStudentRepository`, `IUserRepository`, `ITeacherRepository`
  - `MediaStorageValidator`
  - `ICurrentUserContext` (fix hardcoded BranchId)
  - `IAuditLogService`

- [ ] **APP-172c**: Update MediaService.Upload() implementation
  - Call validator before upload
  - Validate owner exists
  - Use real BranchId from context
  - Add audit logging

**Phase 4: Controller Update (15 min)**
- [ ] **API-173**: Update MediaController.Upload()
  - Add `ownerId`, `ownerType`, `collection`, `mediaType` form parameters
  - Remove hardcoded validation (moved to service)
  - Add proper error handling (400/404/500)

**Phase 5: Repository Extension (Optional, 20 min)**
- [ ] **INF-174**: Add `GetTotalSizeByBranchAsync()` to IMediaRepository
  - Sum all media file sizes for a branch
  - Used by quota validator

**Estimated Time**: ~2.5 hours

---

## 🟡 MEDIUM PRIORITY - Background Processing

### 3. Lead Follow-Up Reminder System
**Priority**: P2 - Medium  
**Story Points**: 3  
**Status**: ❌ Not implemented

**Context**: Intakes have a `FollowUpDate` field. When that date arrives, agents should be notified to follow up with the lead.

**What Needs to Be Built**:

- [ ] **DOM-180**: Create `LeadFollowUpDueDomainEvent`
  - Properties: `IntakeId`, `AgentId`, `FollowUpDate`, `LeadName`

- [ ] **INF-181**: Create `LeadFollowUpReminderProcessor` (Hangfire/HostedService)
  - Runs daily at 9am
  - Query: `Intakes.Where(i => i.FollowUpDate == Today && i.Status != Converted)`
  - Emit `LeadFollowUpDueDomainEvent` for each
  - Mark intake as "FollowUpReminderSent" (optional status flag)

- [ ] **APP-182**: Create `LeadFollowUpDueEventHandler`
  - Send notification to agent (email/SMS/in-app)
  - Log notification in AuditLog

- [ ] **INF-183**: Register background service in Program.cs
  ```csharp
  builder.Services.AddHostedService<LeadFollowUpReminderProcessor>();
  ```

**Business Rules**:
- Only send reminder once per FollowUpDate
- Only for non-converted intakes
- Agent receives notification at 9am on FollowUpDate

**Estimated Time**: ~2 hours

---

### 4. Notification Service (Email/SMS Templates)
**Priority**: P2 - Medium  
**Story Points**: 4  
**Status**: ⚠️ Partial (infrastructure exists, templates missing)

**Context**: System has domain events (invoice issued, payment received, enrollment created), but no notification sending.

**What Needs to Be Built**:

- [ ] **APP-190**: Create `INotificationService` interface
  ```csharp
  Task SendEmailAsync(string to, string subject, string body);
  Task SendSMSAsync(string phone, string message);
  Task SendInvoiceIssuedNotificationAsync(Guid invoiceId);
  Task SendPaymentReceiptAsync(Guid paymentId);
  Task SendEnrollmentCreatedNotificationAsync(Guid enrollmentId);
  Task SendLeadFollowUpReminderAsync(Guid intakeId, Guid agentId);
  ```

- [ ] **APP-191**: Create `NotificationService` implementation
  - Email provider integration (SMTP/SendGrid)
  - SMS provider integration (Twilio/etc.)
  - Template rendering with variables

- [ ] **APP-192**: Create notification templates (Razor/Liquid/HTML)
  - `invoice-issued.html` - Hi {StudentName}, your invoice #{InvoiceNumber} for {Amount} is ready
  - `payment-receipt.html` - Payment of {Amount} received for {EnrollmentSubject}
  - `enrollment-created.html` - Welcome {StudentName}! You're enrolled in {Subject}
  - `lead-followup-reminder.html` - Reminder: Follow up with {LeadName} today

- [ ] **APP-193**: Create event handlers
  - `InvoiceIssuedEventHandler` → calls `SendInvoiceIssuedNotificationAsync()`
  - `PaymentCompletedEventHandler` → calls `SendPaymentReceiptAsync()`
  - `EnrollmentCreatedEventHandler` → calls `SendEnrollmentCreatedNotificationAsync()`
  - `LeadFollowUpDueEventHandler` → calls `SendLeadFollowUpReminderAsync()`

- [ ] **APP-194**: Add notification settings to appsettings.json
  ```json
  {
    "Notifications": {
      "Email": {
        "SmtpHost": "smtp.gmail.com",
        "SmtpPort": 587,
        "FromEmail": "noreply@school.com"
      },
      "SMS": {
        "Provider": "Twilio",
        "AccountSid": "...",
        "AuthToken": "..."
      }
    }
  }
  ```

**Business Rules**:
- All notifications logged in AuditLog
- Failed notifications don't block transactions (fire-and-forget)
- Retry logic for failed sends (optional)

**Estimated Time**: ~3 hours

---

## 🔵 LOW PRIORITY - Nice to Have

### 5. Absence Alert System (Academic Feature)
**Priority**: P3 - Low (Academic feature, may be out of scope)  
**Story Points**: 2  
**Status**: ❌ Not implemented

**Context**: When a student accumulates too many absences, notify parent/guardian.

**Deferred**: This is an academic feature. May be excluded from ERP scope.

---

## 📊 Summary Dashboard

| Feature | Priority | Story Points | Status |
|---------|----------|--------------|--------|
| **Parent/Guardian Management Endpoints** | P1 | 3 | ⚠️ Partial (service done) |
| **Media Storage Governance** | P1 | 3 | ⚠️ Partial (basic upload works) |
| **Lead Follow-Up Reminder System** | P2 | 3 | ❌ Not started |
| **Notification Service (Email/SMS)** | P2 | 4 | ❌ Not started |
| **Absence Alert System** | P3 | 2 | ❌ Deferred (Academic) |

**Total Remaining Story Points**: 13 story points

**P1 (Must Have)**: 6 story points (~1 week)  
**P2 (Should Have)**: 7 story points (~1 week)  
**P3 (Nice to Have)**: Deferred to post-MVP

---

## 🎯 Recommended Execution Order

### This Week (P1 - Critical):
1. **Parent/Guardian Management Endpoints** (3 pts, ~2 hours)
   - Complete CRUD endpoints for StudentResponsable
   - Enable post-registration parent management

2. **Media Storage Governance** (3 pts, ~2.5 hours)
   - Add file validation (size, type, owner)
   - Implement storage quotas
   - Fix hardcoded BranchId

### Next Sprint (P2 - Important):
3. **Lead Follow-Up Reminder System** (3 pts, ~2 hours)
   - Daily background job
   - Agent notifications

4. **Notification Service** (4 pts, ~3 hours)
   - Email/SMS infrastructure
   - Event handlers
   - Templates

### Later (P3 - Optional):
5. **Absence Alert System** (2 pts)
   - Deferred: academic feature, may be out of ERP scope

---

**Document Version**: 1.0  
**Last Updated**: August 1, 2026  
**Next Review**: After completing P1 features

---

## 📝 Notes

**What's NOT in this document**:
- Simple CRUD operations (Branch, Room, Level, Platform, Teacher, Agent, Subject, Plan)
- Financial reporting (revenue, aging, profitability)
- Operational analytics (occupancy, funnel, retention)

**Why separate this?**:
- CRUD operations are straightforward (just scaffolding)
- This document focuses on **complex workflows** that require:
  - Multi-step orchestration
  - Domain validation
  - Cross-entity operations
  - Background processing
  - File handling with governance

**After completing this document**:
- System will have all complex business logic
- CRUD endpoints can be added as needed by frontend
- Reporting/analytics can be added in post-MVP phase
