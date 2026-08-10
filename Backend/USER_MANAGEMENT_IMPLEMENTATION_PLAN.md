# User Management & RBAC Implementation Plan

**Date**: August 1, 2026  
**Purpose**: Complete implementation roadmap for User Management, Roles, Claims, and Branch Assignment

---

## 🔍 Current State Analysis

### ✅ What We Have:
1. **DomainUser Entity** (Person-based) - Exists in Domain
2. **ApplicationUser** (ASP.NET Identity) - Exists in CrossCutting.Identity
3. **CurrentUserContext** - Partially implemented (extracts BranchId and NameIdentifier)
4. **UserRequestDto** - Basic DTO exists
5. **UserRepository** - Basic repository exists
6. **UserQueryService** - Basic query service exists

### ❌ What's Missing:
1. **Role Management** - No entity for storing roles (using ASP.NET Identity roles)
2. **Claims Management** - No custom claims beyond BranchId
3. **Branch Assignment** - DomainUser has no BranchId property
4. **Multi-Branch Support** - No user-to-branches mapping
5. **UserController** - No API endpoints for user management
6. **UserService** - No business logic layer
7. **Role-Based Authorization** - No policies configured
8. **SuperAdmin workflows** - No admin panel endpoints

---

## 📋 Implementation Phases

---

## Phase 1: Domain Layer Changes

### 1.1 Update DomainUser Entity

**File**: `SchoolManagement.Domain/Common/Entities/DomainUser.cs`

**Add Properties:**
- `Email` (string, required)
- `Phone` (string, optional)
- `DateOfBirth` (DateOnly, optional)
- `Role` (string, required) - SuperAdmin, Director, Administrator, Receptionist, Teacher, CommercialAgent
- `BranchId` (Guid?, nullable) - NULL for SuperAdmin only
- `BranchIds` (List<Guid>) - For multi-branch support (future)

**Add Methods:**
- `AssignToBranch(Guid branchId)` - Assign user to branch
- `RemoveFromBranch()` - Clear branch assignment
- `UpdateEmail(string email)` - Change email
- `UpdatePhone(string phone)` - Update phone
- `UpdateRole(string role)` - Change role (SuperAdmin only)
- `AddToBranch(Guid branchId)` - Add additional branch (multi-branch)
- `RemoveFromBranch(Guid branchId)` - Remove from specific branch

**Business Rules:**
- SuperAdmin must have BranchId = NULL
- Other roles must have at least one BranchId
- Role must be one of: SuperAdmin, Director, Administrator, Receptionist, Teacher, CommercialAgent
- Email must be unique
- Cannot remove last branch from user

---

### 1.2 Create UserBranch Entity (Multi-Branch Support)

**File**: `SchoolManagement.Domain/Common/Entities/UserBranch.cs`

**Purpose**: Support users assigned to multiple branches (future)

**Properties:**
- `Id` (Guid)
- `UserId` (Guid, FK to DomainUser)
- `BranchId` (Guid, FK to Branch)
- `IsPrimary` (bool) - One branch must be primary
- `AssignedAt` (DateTime)
- `AssignedBy` (Guid) - Who assigned this user

**Navigation:**
- `User` → DomainUser
- `Branch` → Branch

**Note**: For now, use single BranchId. This table is for future multi-branch feature.

---

### 1.3 Create Role Constants

**File**: `SchoolManagement.Domain/Common/Constants/Roles.cs`

**Content:**
```
public static class Roles
{
    // Admin Panel Access
    public const string SuperAdmin = "SuperAdmin";
    public const string Director = "Director";
    public const string Administrator = "Administrator";
    public const string Receptionist = "Receptionist";
    
    // No Admin Panel
    public const string Teacher = "Teacher";
    public const string CommercialAgent = "CommercialAgent";
    
    // Role Groups
    public const string AdminPanelRoles = "SuperAdmin,Director,Administrator,Receptionist";
    public const string FullAdminRoles = "SuperAdmin,Director";
    public const string AcademicRoles = "SuperAdmin,Director,Administrator";
    public const string FinancialRoles = "SuperAdmin,Director";
}
```

---

### 1.4 Update ICurrentUserContext Interface

**File**: `SchoolManagement.Application/Common/Interfaces/ICurrentUserContext.cs`

**Add Properties:**
- `string Role { get; }` - User's role
- `string Email { get; }` - User's email
- `bool IsSuperAdmin { get; }` - Quick check
- `bool IsDirector { get; }` - Quick check
- `bool IsAdministrator { get; }` - Quick check

---

## Phase 2: Infrastructure Layer Changes

### 2.1 Update CurrentUserContext Implementation

**File**: `SchoolManagement.Infrastructure/Data/CurrentUserContext.cs`

**Changes:**
- Extract "Role" claim from JWT
- Extract "Email" claim from JWT
- Handle SuperAdmin (no BranchId or BranchId = empty GUID)
- Add IsSuperAdmin property (role == "SuperAdmin")
- Don't throw exception if BranchId missing for SuperAdmin

**Logic:**
```
if (role == "SuperAdmin"):
    BranchId = Guid.Empty (or null)
else:
    BranchId = extract from claims (required)
```

---

### 2.2 Update UserConfiguration (EF)

**File**: `SchoolManagement.Infrastructure/Data/Configurations/Entities/UserConfiguration.cs`

**Add Configurations:**
- Index on Email (unique)
- Index on BranchId
- Index on Role
- Required: Email, Role
- Optional: BranchId (for SuperAdmin)
- MaxLength: Email (255), Role (50), Phone (20)
- Relationship: User.BranchId → Branch.Id (optional FK)

---

### 2.3 Update UserRepository

**File**: `SchoolManagement.Infrastructure/Common/Repositories/UserRepository.cs`

**Add Methods:**
- `Task<DomainUser?> GetByEmailAsync(string email)`
- `Task<bool> ExistsByEmailAsync(string email)`
- `Task<List<DomainUser>> GetByBranchAsync(Guid branchId)`
- `Task<List<DomainUser>> GetByRoleAsync(string role)`
- `Task<DomainUser?> GetByApplicationUserIdAsync(string appUserId)`

---

### 2.4 Update UserQueryService

**File**: `SchoolManagement.Infrastructure/Common/Queries/UserQueryService.cs`

**Add Methods:**
- `Task<List<UserResponseDto>> GetAllWithBranchAsync()` - Include branch info
- `Task<UserResponseDto?> GetByIdWithBranchAsync(Guid id)` - Include branch
- `Task<List<UserResponseDto>> GetByBranchAsync(Guid branchId)`
- `Task<List<UserResponseDto>> GetByRoleAsync(string role)`
- `Task<bool> EmailExistsAsync(string email)`

---

## Phase 3: Application Layer Changes

### 3.1 Create Request DTOs

**Files to Create:**

**3.1.1** `CreateUserRequestDto.cs`
- FirstName (required)
- LastName (required)
- Email (required, unique)
- Phone (optional)
- DateOfBirth (optional)
- GenderId (required)
- Role (required, enum/string)
- BranchId (required for non-SuperAdmin) - SuperAdmin selects from dropdown
- Password (required, min 8 chars)

**3.1.2** `UpdateUserRequestDto.cs`
- FirstName
- LastName
- Email
- Phone
- DateOfBirth
- GenderId
- IsActive

**3.1.3** `AssignBranchRequestDto.cs`
- UserId (required)
- BranchId (required)

**3.1.4** `ChangeRoleRequestDto.cs`
- UserId (required)
- NewRole (required)

**3.1.5** `ResetPasswordRequestDto.cs`
- UserId (required)
- NewPassword (required, min 8 chars)

---

### 3.2 Create Response DTOs

**Files to Create:**

**3.2.1** `UserResponseDto.cs`
- Id
- FirstName, LastName
- Email, Phone
- DateOfBirth
- GenderId, GenderName
- Role
- BranchId, BranchName (nullable for SuperAdmin)
- IsActive
- LastActiveAt
- CreatedAt, UpdatedAt

**3.2.2** `UserDetailResponseDto.cs` (extends UserResponseDto)
- Plus: AssignedBranches (List<BranchSummary>) - For multi-branch users
- Plus: Permissions/Claims

---

### 3.3 Create Commands

**Files to Create:**

**3.3.1** `CreateUserCommand.cs`
- All properties from CreateUserRequestDto
- Plus: Slug (generated by service)
- Plus: CreatedBy (Guid) - from CurrentUserContext

**3.3.2** `UpdateUserCommand.cs`
- Similar to UpdateUserRequestDto
- Plus: UpdatedBy (Guid)

**3.3.3** `AssignBranchCommand.cs`
- UserId, BranchId
- AssignedBy (Guid)

**3.3.4** `ChangeRoleCommand.cs`
- UserId, NewRole, OldRole
- ChangedBy (Guid)

---

### 3.4 Create UserMapper

**File**: `SchoolManagement.Application/Common/Mappers/UserMapper.cs`

**Methods:**
- `ToDomain(CreateUserCommand)` → DomainUser
- `ToResponse(DomainUser)` → UserResponseDto
- `ToDetailResponse(DomainUser)` → UserDetailResponseDto

---

### 3.5 Create IUserService Interface

**File**: `SchoolManagement.Application/Common/Interfaces/Services/IUserService.cs`

**Methods:**
- `Task<UserResponseDto> CreateAsync(CreateUserCommand command)`
- `Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserCommand command)`
- `Task DeleteAsync(Guid id)`
- `Task<UserResponseDto> GetByIdAsync(Guid id)`
- `Task<List<UserResponseDto>> GetAllAsync()`
- `Task<List<UserResponseDto>> GetByBranchAsync(Guid branchId)`
- `Task<List<UserResponseDto>> GetByRoleAsync(string role)`
- `Task AssignToBranchAsync(AssignBranchCommand command)`
- `Task RemoveFromBranchAsync(Guid userId, Guid branchId)`
- `Task ChangeRoleAsync(ChangeRoleCommand command)`
- `Task<UserResponseDto> ActivateAsync(Guid userId)`
- `Task<UserResponseDto> DeactivateAsync(Guid userId)`
- `Task ResetPasswordAsync(ResetPasswordCommand command)`

---

### 3.6 Create UserService Implementation

**File**: `SchoolManagement.Application/Common/Services/UserService.cs`

**Dependencies:**
- IUserRepository
- IUserQueryService
- IBranchRepository
- ICurrentUserContext
- IAuditLogService
- IPasswordHasher (from Identity)
- UserManager<ApplicationUser> (from Identity)

**Business Logic:**

**CreateAsync:**
1. Validate: Email is unique
2. Validate: Role is valid
3. Validate: If not SuperAdmin, BranchId is required
4. Validate: If SuperAdmin, BranchId must be null
5. Generate slug (FirstName-LastName-Email)
6. Hash password
7. Create ApplicationUser (ASP.NET Identity)
8. Create DomainUser with ApplicationUserId
9. Assign role to ApplicationUser
10. Add claims: UserId, BranchId, Role, Email
11. Audit log
12. Return response

**UpdateAsync:**
1. Get existing user
2. Check permissions (SuperAdmin or own profile)
3. Validate branch ownership
4. Update properties
5. Audit log

**AssignToBranchAsync:**
1. Validate: Only SuperAdmin or Director can do this
2. Validate: Branch exists
3. Validate: User cannot be SuperAdmin
4. Update user.BranchId
5. Update JWT claims (user must re-login)
6. Audit log

**ChangeRoleAsync:**
1. Validate: Only SuperAdmin can change roles
2. Validate: New role is valid
3. Update user.Role
4. Update ApplicationUser role
5. Update JWT claims (user must re-login)
6. Audit log

**ResetPasswordAsync:**
1. Validate: Only SuperAdmin or own password
2. Hash new password
3. Update ApplicationUser password
4. Force logout (invalidate tokens)
5. Audit log

---

## Phase 4: API Layer Changes

### 4.1 Create UserController

**File**: `SchoolManagement.Api/Controllers/UserController.cs`

**Endpoints:**

#### 4.1.1 GET /api/users
- **Authorization**: SuperAdmin, Director
- **Logic**: 
  - SuperAdmin: All users across all branches
  - Director: Users in their branch only
- **Returns**: List<UserResponseDto>

#### 4.1.2 GET /api/users/{id}
- **Authorization**: SuperAdmin, Director, (own profile)
- **Logic**: Get user by ID with branch info
- **Returns**: UserDetailResponseDto

#### 4.1.3 POST /api/users
- **Authorization**: SuperAdmin, Director
- **Body**: CreateUserRequestDto
- **Logic**:
  - SuperAdmin: Can create for any branch, select role
  - Director: Can create for their branch only, select role (except SuperAdmin)
  - Map to Command, call UserService.CreateAsync
- **Returns**: UserResponseDto

#### 4.1.4 PUT /api/users/{id}
- **Authorization**: SuperAdmin, Director, (own profile)
- **Body**: UpdateUserRequestDto
- **Logic**: Update user details
- **Returns**: UserResponseDto

#### 4.1.5 DELETE /api/users/{id}
- **Authorization**: SuperAdmin only
- **Logic**: Soft delete (deactivate) user
- **Returns**: 204 No Content

#### 4.1.6 POST /api/users/{id}/assign-branch
- **Authorization**: SuperAdmin only
- **Body**: { "branchId": "guid" }
- **Logic**: Assign/reassign user to branch
- **Returns**: UserResponseDto

#### 4.1.7 POST /api/users/{id}/change-role
- **Authorization**: SuperAdmin only
- **Body**: { "newRole": "Director" }
- **Logic**: Change user's role
- **Returns**: UserResponseDto

#### 4.1.8 POST /api/users/{id}/reset-password
- **Authorization**: SuperAdmin, (own password)
- **Body**: { "newPassword": "..." }
- **Logic**: Reset user password
- **Returns**: 200 OK

#### 4.1.9 POST /api/users/{id}/activate
- **Authorization**: SuperAdmin, Director
- **Logic**: Activate deactivated user
- **Returns**: UserResponseDto

#### 4.1.10 POST /api/users/{id}/deactivate
- **Authorization**: SuperAdmin, Director
- **Logic**: Deactivate user (soft delete)
- **Returns**: UserResponseDto

#### 4.1.11 GET /api/users/branch/{branchId}
- **Authorization**: SuperAdmin, Director (own branch)
- **Logic**: Get all users in specific branch
- **Returns**: List<UserResponseDto>

#### 4.1.12 GET /api/users/role/{role}
- **Authorization**: SuperAdmin, Director
- **Logic**: Get all users with specific role
- **Returns**: List<UserResponseDto>

---

## Phase 5: Authentication & JWT Changes

### 5.1 Update Login Flow

**File**: `SchoolManagement.CrossCutting.Identity/Services/AuthService.cs` (or similar)

**Changes to JWT Token Generation:**

**Add Claims:**
1. `sub` → User ID (GUID)
2. `nameid` → User ID (GUID)
3. `email` → User email
4. `role` → User role (SuperAdmin, Director, etc.)
5. `branchId` → User's branch (empty for SuperAdmin)
6. `exp` → Expiration

**Login Logic:**
1. Validate credentials
2. Get DomainUser by ApplicationUserId
3. Check user.IsActive
4. Extract: Role, BranchId
5. Generate JWT with all claims
6. Update user.LastActiveAt
7. Return token

**Example JWT Payload:**
```json
{
  "sub": "user-guid",
  "nameid": "user-guid",
  "email": "director@branch1.com",
  "role": "Director",
  "branchId": "branch-1-guid",
  "exp": 1234567890
}
```

---

### 5.2 Update Register Flow (if applicable)

**Note**: Users should NOT self-register. Only SuperAdmin/Director create users.

If public registration exists, disable it or restrict to specific roles only.

---

## Phase 6: Authorization Policies

### 6.1 Create Authorization Policies

**File**: `Program.cs` or `Startup.cs`

**Add Policies:**

```
services.AddAuthorization(options =>
{
    // Admin Panel Access
    options.AddPolicy("AdminPanelAccess", policy => 
        policy.RequireRole(Roles.SuperAdmin, Roles.Director, Roles.Administrator, Roles.Receptionist));
    
    // Full Admin
    options.AddPolicy("FullAdmin", policy => 
        policy.RequireRole(Roles.SuperAdmin, Roles.Director));
    
    // Academic Operations
    options.AddPolicy("AcademicAccess", policy => 
        policy.RequireRole(Roles.SuperAdmin, Roles.Director, Roles.Administrator));
    
    // Financial Operations
    options.AddPolicy("FinancialAccess", policy => 
        policy.RequireRole(Roles.SuperAdmin, Roles.Director));
    
    // Branch Required
    options.AddPolicy("HasBranch", policy => 
        policy.RequireAssertion(context => 
            context.User.HasClaim(c => c.Type == "branchId") || 
            context.User.IsInRole(Roles.SuperAdmin)));
});
```

---

### 6.2 Create Custom Authorization Handlers

**Files to Create:**

**6.2.1** `RequireBranchAttribute.cs` - Custom filter
- Validates non-SuperAdmin users have valid BranchId claim

**6.2.2** `RequireOwnershipOrRoleHandler.cs` - Custom handler
- Validates user owns resource OR has required role

**6.2.3** `RequireTeacherAssignmentHandler.cs` - Custom handler
- Validates teacher is assigned to group

---

## Phase 7: Database Migration

### 7.1 Add Migration

**Command:**
```bash
dotnet ef migrations add AddUserManagement
```

**Changes:**
- Add BranchId column to DomainUser (nullable)
- Add Role column to DomainUser (required)
- Add Email column to DomainUser (required, unique index)
- Add Phone column to DomainUser (optional)
- Add DateOfBirth column to DomainUser (optional)
- Add FK: DomainUser.BranchId → Branch.Id (optional)
- Create indexes on Email, Role, BranchId

---

### 7.2 Update Database

**Command:**
```bash
dotnet ef database update
```

---

## Phase 8: Seeding & Initial Data

### 8.1 Create SuperAdmin Seeder

**File**: `SchoolManagement.Infrastructure/Data/Seeders/SuperAdminSeeder.cs`

**Seed Data:**
1. Create ApplicationUser:
   - Email: superadmin@school.com
   - Password: (hashed) - e.g., "SuperAdmin@123"
   - Role: SuperAdmin

2. Create DomainUser:
   - FirstName: Super
   - LastName: Admin
   - Email: superadmin@school.com
   - Role: SuperAdmin
   - BranchId: NULL
   - ApplicationUserId: (link to ApplicationUser)
   - IsActive: true
   - Slug: super-admin-superadmin-school-com

3. Assign "SuperAdmin" role to ApplicationUser

**Note**: Run this seed only once during initial setup.

---

### 8.2 (Optional) Create Sample Users

**For Testing Purposes:**

Create 2 branches:
- Branch A (Main Campus)
- Branch B (East Campus)

Create users:
1. Director (Branch A)
2. Administrator (Branch A)
3. Receptionist (Branch A)
4. Teacher (Branch A)
5. Director (Branch B)
6. Receptionist (Branch B)

---

## Phase 9: Frontend Integration

### 9.1 Login Page
- User enters email/password
- Backend returns JWT with all claims
- Frontend stores JWT in localStorage
- Frontend decodes JWT to get: role, branchId, email

### 9.2 Admin Panel - User Management

**For SuperAdmin:**
- View all users across all branches
- Create user → Select Branch from dropdown
- Edit user → Can change Branch
- Change user role
- Activate/Deactivate users
- Reset passwords

**For Director:**
- View users in their branch only
- Create user → Auto-assigned to Director's branch (no dropdown)
- Edit user → Cannot change Branch
- Cannot change user role to SuperAdmin
- Activate/Deactivate users in their branch
- Reset passwords for their branch users

**UI Components:**
- User List Table (filterable by branch, role, status)
- Create User Form (with role & branch selection)
- Edit User Form
- Assign Branch Modal (SuperAdmin only)
- Change Role Modal (SuperAdmin only)
- Reset Password Modal

---

### 9.3 UI Navigation

**Show/Hide Based on Role:**
- User Management → AdminPanelAccess policy
- Branch Management → SuperAdmin only
- Financial Reports → FinancialAccess policy
- Academic Operations → AcademicAccess policy

**Display Current User Info:**
- Show user's branch name (if not SuperAdmin)
- Show "All Branches" for SuperAdmin
- Show role badge
- Profile menu with logout

---

## Phase 10: Testing

### 10.1 Unit Tests

**Test Cases:**
1. Create user with valid data → Success
2. Create user with duplicate email → Fail
3. Create non-SuperAdmin without BranchId → Fail
4. Create SuperAdmin with BranchId → Fail
5. SuperAdmin views all users → Returns all
6. Director views users → Returns own branch only
7. Assign user to branch (SuperAdmin) → Success
8. Assign user to branch (Director) → Fail
9. Change user role (SuperAdmin) → Success
10. Change user role (Director) → Fail

### 10.2 Integration Tests

**Test Scenarios:**
1. SuperAdmin creates Director for Branch A → Success
2. Director (Branch A) creates Receptionist → Auto-assigned to Branch A
3. Director (Branch A) tries to view Branch B users → 403 Forbidden
4. Teacher (Branch A) tries to view users → 403 Forbidden
5. User re-login after branch change → New JWT with new branchId

### 10.3 Manual Testing

**Workflows to Test:**
1. SuperAdmin creates branch
2. SuperAdmin creates Director for that branch
3. Director logs in
4. Director creates Administrator, Receptionist
5. Director views only their branch staff
6. SuperAdmin reassigns Director to different branch
7. Director logs out and logs in → Now sees new branch data

---

## Phase 11: Documentation

### 11.1 API Documentation

**Update Swagger/OpenAPI:**
- Document all /api/users endpoints
- Add authorization requirements
- Add request/response examples

### 11.2 Admin Guide

**Create Document:**
- How to create users
- How to assign branches
- How to change roles
- How to reset passwords
- How to deactivate users

---

## 📋 Implementation Checklist

### Phase 1: Domain
- [ ] Update DomainUser entity (add Email, Phone, Role, BranchId)
- [ ] Create Roles constants class
- [ ] Update ICurrentUserContext interface

### Phase 2: Infrastructure
- [ ] Update CurrentUserContext (extract Role claim, handle SuperAdmin)
- [ ] Update UserConfiguration (EF mapping)
- [ ] Add UserRepository methods (GetByEmail, GetByBranch, etc.)
- [ ] Update UserQueryService

### Phase 3: Application
- [ ] Create Request DTOs (Create, Update, AssignBranch, ChangeRole, ResetPassword)
- [ ] Create Response DTOs (UserResponseDto, UserDetailResponseDto)
- [ ] Create Commands
- [ ] Create UserMapper
- [ ] Create IUserService interface
- [ ] Implement UserService

### Phase 4: API
- [ ] Create UserController with all endpoints
- [ ] Add authorization attributes

### Phase 5: Authentication
- [ ] Update JWT generation (add role, branchId claims)
- [ ] Update Login flow

### Phase 6: Authorization
- [ ] Add authorization policies
- [ ] Create custom authorization handlers/filters

### Phase 7: Database
- [ ] Create migration (Add columns to DomainUser)
- [ ] Run migration

### Phase 8: Seeding
- [ ] Create SuperAdmin seeder
- [ ] (Optional) Create sample users seeder

### Phase 9: Frontend
- [ ] Update login page
- [ ] Create User Management UI (SuperAdmin)
- [ ] Create User Management UI (Director)
- [ ] Add role-based navigation

### Phase 10: Testing
- [ ] Write unit tests
- [ ] Write integration tests
- [ ] Manual testing

### Phase 11: Documentation
- [ ] Update API documentation
- [ ] Create admin guide

---

## 🎯 Priority Order

### Critical (Do First):
1. Domain changes (DomainUser entity)
2. Database migration
3. Seed SuperAdmin
4. Update JWT generation
5. Update CurrentUserContext
6. Create UserService
7. Create UserController

### Important (Do Next):
8. Authorization policies
9. Frontend User Management UI
10. Testing

### Nice to Have (Do Later):
11. Multi-branch support (UserBranch table)
12. Advanced permissions
13. Audit trail UI

---

## ⏱️ Estimated Timeline

- **Phase 1-4**: 2-3 days (Backend)
- **Phase 5-7**: 1 day (Auth & Database)
- **Phase 8**: 1 hour (Seeding)
- **Phase 9**: 2-3 days (Frontend)
- **Phase 10**: 1-2 days (Testing)
- **Phase 11**: 1 day (Documentation)

**Total**: ~7-10 days for complete implementation

---

## 🚀 Quick Start (MVP)

**If you need to get started quickly, implement in this order:**

1. Update DomainUser entity (add Role, BranchId)
2. Run migration
3. Seed SuperAdmin
4. Update JWT (add role, branchId claims)
5. Update CurrentUserContext (handle SuperAdmin)
6. Create basic UserService (CreateAsync, GetAllAsync)
7. Create basic UserController (GET, POST)
8. Test: SuperAdmin creates Director for Branch A
9. Test: Director logs in and sees BranchId in token

**This gives you a working foundation. Then add other features incrementally.**

---

**This implementation plan provides a complete roadmap for building the User Management & RBAC system with branch isolation. Follow the phases in order for smooth implementation.**
