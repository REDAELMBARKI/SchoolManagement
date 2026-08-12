# Authorization Analysis for All Controllers

## Current Policies Available:
1. **IsSuperAdmin** - SuperAdmin only
2. **IsDirectorOrAbove** - SuperAdmin, Director
3. **IsAdministratorOrAbove** - SuperAdmin, Director, Administrator
4. **IsReceptionistOrAbove** - SuperAdmin, Director, Administrator, Receptionist
5. **CanManageRole** - Resource-based: checks role hierarchy
6. **IsSameBranch** - Resource-based: checks BranchId (Guid)
7. **SelfOrSuperAdmin** - Resource-based: self or SuperAdmin

---

## ✅ COMPLETED (Fully Authorized):

### 1. AccountController
- **Location:** `Controllers/Auth/AccountController.cs`
- **Status:** ✅ COMPLETE
- **Policies Used:** All policies + branch/role checks
- **No new policies needed**

### 2. DomainUserController
- **Location:** `Controllers/DomainUserController.cs`
- **Status:** ✅ COMPLETE
- **Policies Used:** IsAdministratorOrAbove, IsDirectorOrAbove, IsSuperAdmin, CanManageRole, IsSameBranch
- **No new policies needed**

### 3. IntakeController
- **Location:** `Controllers/IntakeController.cs`
- **Status:** ✅ COMPLETE
- **Policies Used:** IsReceptionistOrAbove, IsDirectorOrAbove, IsSameBranch
- **Entity:** Intake (has BranchId)
- **No new policies needed**

---

## 🟡 NEEDS AUTHORIZATION (Uses Existing Policies):

### 4. StudentController ⭐ HIGH PRIORITY
- **Location:** `Controllers/StudentController.cs`
- **Entity:** Student (has BranchId via Intake/Enrollment)
- **Endpoints:** GetAll, GetById, Create, Update, Delete, TransferBranch, GetParents, AddParent, RemoveParent
- **Suggested Policies:**
  - GetAll, GetById: `IsReceptionistOrAbove`
  - Create, Update: `IsReceptionistOrAbove` + `IsSameBranch`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
  - TransferBranch: `IsSuperAdmin` (cross-branch operation)
  - Parents operations: `IsReceptionistOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 5. EnrollmentController ⭐ HIGH PRIORITY
- **Location:** `Controllers/EnrollmentController.cs`
- **Entity:** Enrollment (has BranchId)
- **Endpoints:** GetAll, GetById, Create, Update, Delete, Drop, Complete, TransferGroup, EnrollStudentInAdditionalGroup, CheckScheduleConflicts
- **Suggested Policies:**
  - GetAll, GetById: `IsReceptionistOrAbove`
  - Create, Update: `IsReceptionistOrAbove` + `IsSameBranch`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
  - Drop, Complete, TransferGroup: `IsReceptionistOrAbove` + `IsSameBranch`
  - EnrollStudentInAdditionalGroup: `IsReceptionistOrAbove` + `IsSameBranch`
  - CheckScheduleConflicts: `IsReceptionistOrAbove` (read-only check)
- **✅ NO NEW POLICIES NEEDED**

---

### 6. GroupController
- **Location:** `Controllers/GroupController.cs`
- **Entity:** Group (has BranchId)
- **Endpoints:** GetAll, GetById, Create, Update, Delete
- **Suggested Policies:**
  - GetAll, GetById: `IsReceptionistOrAbove`
  - Create, Update: `IsAdministratorOrAbove` + `IsSameBranch`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 7. ScheduleController
- **Location:** `Controllers/ScheduleController.cs`
- **Entity:** Schedule (has BranchId)
- **Endpoints:** CreateSchedules, GetGroupSchedule, UpdateSchedule, DeleteSchedule, CheckRoomAvailability, CheckTeacherAvailability
- **Suggested Policies:**
  - GetGroupSchedule: `IsReceptionistOrAbove`
  - CreateSchedules, UpdateSchedule: `IsAdministratorOrAbove` + `IsSameBranch`
  - DeleteSchedule: `IsDirectorOrAbove` + `IsSameBranch`
  - CheckRoomAvailability, CheckTeacherAvailability: `IsReceptionistOrAbove` (validation checks)
- **✅ NO NEW POLICIES NEEDED**

---

### 8. InvoiceController
- **Location:** `Controllers/InvoiceController.cs`
- **Entity:** Invoice (has BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsReceptionistOrAbove`
  - Create, Update: `IsReceptionistOrAbove` + `IsSameBranch`
  - Delete/Cancel: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 9. PaymentController
- **Location:** `Controllers/PaymentController.cs`
- **Entity:** Payment (has BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsReceptionistOrAbove`
  - Create, Update: `IsReceptionistOrAbove` + `IsSameBranch`
  - Delete/Refund: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 10. ExpenseController
- **Location:** `Controllers/ExpenseController.cs`
- **Entity:** Expense (has BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsAdministratorOrAbove`
  - Create, Update: `IsAdministratorOrAbove` + `IsSameBranch`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 11. PayrollPaymentController
- **Location:** `Controllers/PayrollPaymentController.cs`
- **Entity:** PayrollPayment (has BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsAdministratorOrAbove`
  - Create, Update: `IsDirectorOrAbove` + `IsSameBranch`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 12. TeacherController
- **Location:** `Controllers/TeacherController.cs`
- **Entity:** Teacher (Employee with BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsAdministratorOrAbove`
  - Create, Update: `IsDirectorOrAbove` + `IsSameBranch`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 13. CommercialAgentController
- **Location:** `Controllers/CommercialAgentController.cs`
- **Entity:** CommercialAgent (Employee with BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsAdministratorOrAbove`
  - Create, Update: `IsDirectorOrAbove` + `IsSameBranch`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 14. CommissionController
- **Location:** `Controllers/CommissionController.cs`
- **Entity:** Commission (related to CommercialAgent)
- **Suggested Policies:**
  - GetAll, GetById: `IsAdministratorOrAbove`
  - Create, Update, Delete: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 15. RefundController
- **Location:** `Controllers/RefundController.cs`
- **Entity:** Refund (related to Payment/Invoice with BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsAdministratorOrAbove`
  - Create, Update: `IsDirectorOrAbove` + `IsSameBranch`
  - Delete/Approve: `IsDirectorOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

---

### 16. GradeController
- **Location:** `Controllers/GradeController.cs`
- **Entity:** Grade (has BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsReceptionistOrAbove` (students can view via different endpoint)
  - Create, Update: Teachers for their groups + `IsAdministratorOrAbove`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
- **⚠️ MAY NEED NEW POLICY:** `IsTeacherForThisGroup` or `CanManageGradesForGroup`

---

### 17. AbsenceController
- **Location:** `Controllers/AbsenceController.cs`
- **Entity:** Absence (has BranchId)
- **Suggested Policies:**
  - GetAll, GetById: `IsReceptionistOrAbove`
  - Create, Update: Teachers for their groups + `IsAdministratorOrAbove`
  - Delete: `IsDirectorOrAbove` + `IsSameBranch`
- **⚠️ MAY NEED NEW POLICY:** `IsTeacherForThisSchedule` or `CanManageAbsences`

---

## 🔴 REFERENCE DATA (Low Priority - Public or Shared):

### 18. BranchController
- **Entity:** Branch
- **Note:** Likely needs only `IsSuperAdmin` for CRUD, read-only for others
- **✅ NO NEW POLICIES NEEDED**

### 19. GenderController
- **Entity:** Gender (lookup/reference)
- **Suggested:** Public read, `IsAdministratorOrAbove` for write
- **✅ NO NEW POLICIES NEEDED**

### 20. LevelController
- **Entity:** Level (has BranchId)
- **Suggested:** `IsReceptionistOrAbove` read, `IsAdministratorOrAbove` write
- **✅ NO NEW POLICIES NEEDED**

### 21. SubjectController
- **Entity:** Subject (has BranchId)
- **Suggested:** `IsReceptionistOrAbove` read, `IsAdministratorOrAbove` write
- **✅ NO NEW POLICIES NEEDED**

### 22. RoomController
- **Entity:** Room (has BranchId)
- **Suggested:** `IsReceptionistOrAbove` read, `IsAdministratorOrAbove` write
- **✅ NO NEW POLICIES NEEDED**

### 23. PlanController
- **Entity:** Plan (pricing plans)
- **Suggested:** `IsReceptionistOrAbove` read, `IsDirectorOrAbove` write
- **✅ NO NEW POLICIES NEEDED**

### 24. CommissionTierController
- **Entity:** CommissionTier
- **Suggested:** `IsAdministratorOrAbove` read, `IsDirectorOrAbove` write
- **✅ NO NEW POLICIES NEEDED**

### 25. AdController, LeadSourceController, OpcController, PlatformController
- **Entities:** Marketing/tracking entities (have BranchId)
- **Suggested:** `IsReceptionistOrAbove` read, `IsAdministratorOrAbove` write
- **✅ NO NEW POLICIES NEEDED**

### 26. MediaController
- **Entity:** Media (has BranchId)
- **Suggested:** `IsReceptionistOrAbove` for uploads, branch isolation
- **✅ NO NEW POLICIES NEEDED**

### 27. WhatsAppController
- **Entity:** WhatsAppMessage (has BranchId)
- **Suggested:** `IsReceptionistOrAbove` + `IsSameBranch`
- **✅ NO NEW POLICIES NEEDED**

### 28. StudentRegistrationController
- **Note:** Public registration endpoint
- **Suggested:** `[AllowAnonymous]` for registration, admin endpoints need auth
- **✅ NO NEW POLICIES NEEDED**

---

## 🔴 NEW POLICIES NEEDED (Only 2!):

### Policy 1: IsTeacherForThisGroup
**Purpose:** Allow teachers to manage grades/absences for their assigned groups only
**Usage:** GradeController, AbsenceController
**Implementation:**
```csharp
public class IsTeacherForGroupRequirement : IAuthorizationRequirement
{
    public Guid GroupId { get; set; }
}

public class IsTeacherForGroupHandler : AuthorizationHandler<IsTeacherForGroupRequirement, Guid>
{
    // Check if current user is Teacher AND assigned to this group
    // OR user is Administrator/Director/SuperAdmin (can override)
}
```

### Policy 2: IsTeacherOrAbove (Optional)
**Purpose:** Simple role-based policy for Teacher and above
**Usage:** Various read operations where teachers need access
**Implementation:**
```csharp
options.AddPolicy("IsTeacherOrAbove", policy => 
    policy.RequireRole("SuperAdmin", "Director", "Administrator", "Teacher"));
```

---

## 📊 Summary:

- **Total Controllers:** 28+
- **Already Authorized:** 3 (Account, DomainUser, Intake)
- **Need Authorization (Existing Policies):** 23 controllers
- **New Policies Required:** 2 only (Teacher-specific)

## 🎯 Recommended Implementation Order:

### Phase 1: HIGH PRIORITY (Core Operations)
1. ✅ IntakeController (DONE)
2. **StudentController** ⭐
3. **EnrollmentController** ⭐
4. **InvoiceController**
5. **PaymentController**

### Phase 2: MEDIUM PRIORITY (Academic)
6. **GroupController**
7. **ScheduleController**
8. **GradeController** (needs new policy)
9. **AbsenceController** (needs new policy)
10. **TeacherController**

### Phase 3: LOW PRIORITY (Reference & Admin)
11. All remaining controllers (use existing policies)

---

## ✅ CONCLUSION:

**You have 95% coverage with existing policies!** Only 2 new policies needed for teacher-specific operations.

All other controllers can be secured using:
- `IsReceptionistOrAbove` (most read/write operations)
- `IsAdministratorOrAbove` (sensitive operations)
- `IsDirectorOrAbove` (delete/financial operations)
- `IsSuperAdmin` (cross-branch, critical operations)
- `IsSameBranch` (branch isolation on all resources)
- `CanManageRole` (user management)
