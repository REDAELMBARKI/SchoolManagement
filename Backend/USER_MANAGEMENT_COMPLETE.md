# ✅ User Management Refactoring - COMPLETE

## Overview
Clean separation between Authentication (Identity layer) and Business Logic (Application layer).

---

## Architecture

### Two User Types:

1. **ApplicationUser** (Identity Layer - ASP.NET Identity)
   - Stored in AspNetUsers table
   - Email, Password, Roles, Claims
   - Used for authentication/login
   - Managed by AuthService

2. **DomainUser** (Business Layer - Domain)
   - Stored in DomainUsers table
   - FirstName, LastName, Phone, DateOfBirth, BranchId
   - Business data for staff members
   - Managed by DomainUserService

---

## Services

### AuthService (CrossCutting.Identity)
**Location:** `SchoolManagement.CrossCutting.Identity/Services/AuthService.cs`

**Purpose:** Authentication operations - wraps UserManager<ApplicationUser>

**Methods:**
- `CreateUserAsync(email, password, role)` → Returns ApplicationUserId
- `AuthenticateAsync(email, password)`
- `ChangePasswordAsync`, `ResetPasswordAsync`, `GeneratePasswordResetTokenAsync`
- `AssignRoleAsync`, `ChangeRoleAsync`, `GetUserRolesAsync`
- `AddClaimAsync`, `RemoveClaimAsync`, `GetUserClaimsAsync`
- `ConfirmEmailAsync`, `GenerateEmailConfirmationTokenAsync`

---

### DomainUserService (Application)
**Location:** `SchoolManagement.Application/Common/Services/DomainUserService.cs`

**Purpose:** Business operations - NO UserManager dependency

**Methods:**
- `CreateAsync(DomainUserCommand)` → Creates DomainUser (requires ApplicationUserId)
- `UpdateAsync(id, UpdateDomainUserCommand)`
- `DeleteAsync(id)`
- `GetByIdAsync(id)`, `GetAllAsync()`, `GetByBranchIdAsync(branchId)`, `GetByRoleAsync(role)`
- `AssignBranchAsync(userId, command)`, `RemoveBranchAsync(userId)`
- `ActivateAsync(userId)`, `DeactivateAsync(userId)`

---

## Controllers

### AccountController (Auth Operations)
**Location:** `SchoolManagement.Api/Controllers/Auth/AccountController.cs`

**Endpoints:**

#### Public Registration
```
POST /api/account/register
Body: { email, password }
- No authentication required
- Creates ApplicationUser ONLY (role: "User")
- For students/parents
```

#### Staff Creation
```
POST /api/account/create-staff-user
Body: { email, password, role, firstName, lastName, phone, branchId, ... }
Authorization: Required (SuperAdmin/Director)
- Creates ApplicationUser + DomainUser together
- BranchId is REQUIRED
- Cannot create SuperAdmin (only one seeded)
```

#### Other Auth Endpoints
- `POST /api/account/login`
- `POST /api/account/change-password`
- `POST /api/account/forgot-password`
- `POST /api/account/reset-password`
- `POST /api/account/confirm-email`
- `PUT /api/account/{userId}/role`
- `POST /api/account/{userId}/claims`
- `DELETE /api/account/{userId}/claims/{claimType}`
- `GET /api/account/{userId}/claims`
- `GET /api/account/{userId}/roles`

---

### DomainUserController (Business Operations)
**Location:** `SchoolManagement.Api/Controllers/DomainUserController.cs`

**Endpoints:**
- `GET /api/domain-users` - Get all users
- `GET /api/domain-users/{id}` - Get user by ID
- `PUT /api/domain-users/{id}` - Update user profile
- `DELETE /api/domain-users/{id}` - Delete user (soft delete)
- `POST /api/domain-users/{id}/assign-branch` - Assign user to branch
- `POST /api/domain-users/{id}/remove-branch` - Remove user from branch
- `POST /api/domain-users/{id}/activate` - Activate user
- `POST /api/domain-users/{id}/deactivate` - Deactivate user
- `GET /api/domain-users/branch/{branchId}` - Get users by branch
- `GET /api/domain-users/role/{role}` - Get users by role

---

## Commands & DTOs

### Commands
- `DomainUserCommand` - Create DomainUser (BranchId REQUIRED)
- `UpdateDomainUserCommand` - Update DomainUser profile
- `AssignBranchCommand` - Assign branch
- `ChangeRoleCommand` - Change role
- `ResetPasswordCommand` - Reset password

### Request DTOs
- `RegisterUserRequestDto` - Staff creation (full user data)
- `UpdateUserRequestDto` - Update profile
- `AssignBranchRequestDto` - Assign branch
- `LoginRequestDto` - Login credentials
- `ChangePasswordRequestDto` - Change password
- `ForgotPasswordRequestDto` - Forgot password
- `ResetPasswordWithTokenRequestDto` - Reset with token
- `ConfirmEmailRequestDto` - Email confirmation
- `AddClaimRequestDto` - Add claim

### Response DTOs
- `UserResponseDto` - User data with branch/gender names

---

## Key Rules

### BranchId Rules:
- ✅ **REQUIRED** for all staff (non-nullable)
- ❌ **SuperAdmin CANNOT be created via API** (only seeded in database)
- ✅ **One SuperAdmin** - seeded with BranchId = NULL
- ✅ All other users MUST have BranchId

### Role Rules:
- **SuperAdmin**: BranchId = NULL, sees all branches (only seeded)
- **Director**: Single branch, full financial + academic
- **Administrator**: Single branch, academic only
- **Receptionist**: Single branch, intakes + attendance + registration
- **Teacher**: Assigned groups only (NO admin panel)
- **CommercialAgent**: Own leads only (NO admin panel)

### Authorization Rules:
- **SuperAdmin**: Can create/modify all users (except SuperAdmin)
- **Director**: Can create/modify users in THEIR branch only (cannot create Director or SuperAdmin)
- **Others**: Can only update their own profile

---

## Audit Log

### BranchId in Audit Log:
- **REQUIRED** - tracks WHERE the change happened (affected entity's branch)
- **NOT the user's branch**, but the **entity's branch**
- Example: SuperAdmin edits user in Branch A → AuditLog.BranchId = Branch A
- For DomainUser without branch (SuperAdmin): Audit logging is skipped

---

## Registration Flow

### Public User (Student/Parent):
```
POST /api/account/register { email, password }
  ↓
AuthService.CreateUserAsync(email, password, "User")
  ↓
ApplicationUser created (can login, but NOT a staff member)
```

### Staff User (Admin, Director, etc.):
```
POST /api/account/create-staff-user 
{ email, password, role, firstName, lastName, branchId, ... }
  ↓
Step 1: AuthService.CreateUserAsync(email, password, role)
  → Returns ApplicationUserId
  ↓
Step 2: DomainUserService.CreateAsync(DomainUserCommand)
  → Creates DomainUser linked to ApplicationUserId
  ↓
Both ApplicationUser + DomainUser created
```

---

## DI Registration (Program.cs)

```csharp
// Identity layer
builder.Services.AddScoped<SchoolManagement.CrossCutting.Identity.Interfaces.IAuthService, 
                          SchoolManagement.CrossCutting.Identity.Services.AuthService>();

// Business layer
builder.Services.AddScoped<IDomainUserService, DomainUserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
```

---

## Files Changed

### Created:
- `SchoolManagement.CrossCutting.Identity/Interfaces/IAuthService.cs`
- `SchoolManagement.CrossCutting.Identity/Services/AuthService.cs`
- `SchoolManagement.Api/Controllers/Auth/AccountController.cs`
- `SchoolManagement.Api/Controllers/DomainUserController.cs`
- `SchoolManagement.Application/Common/Dtos/Commands/DomainUserCommand.cs`
- `SchoolManagement.Application/Common/Dtos/Commands/UpdateDomainUserCommand.cs`
- Auth DTOs: LoginRequestDto, RegisterUserRequestDto, ChangePasswordRequestDto, etc.

### Renamed:
- `IUserService` → `IDomainUserService`
- `UserService` → `DomainUserService`
- `UserCommand` → `DomainUserCommand`
- `UpdateUserCommand` → `UpdateDomainUserCommand`
- `UserController` → `DomainUserController`

### Modified:
- `SchoolManagement.Application/Common/Services/DomainUserService.cs` - Removed UserManager
- `SchoolManagement.Application/Common/Interfaces/Services/IAuditLogService.cs` - BranchId required
- `SchoolManagement.Infrastructure/Common/Services/AuditLogService.cs` - Updated logic
- `SchoolManagement.Api/Program.cs` - DI registration

---

## Testing

### Build Status: ✅ SUCCESS
```
dotnet build SchoolManagement.Api/SchoolManagement.Api.csproj
Build succeeded with 17 warning(s) in 5.9s
```

### Next Steps:
1. Test AccountController.Register (public)
2. Test AccountController.CreateStaffUser (admin)
3. Update JWT generation to include BranchId claim
4. Update CurrentUserContext to extract Role from JWT
5. Add authentication middleware
6. Test all endpoints with Postman/HTTP files

---

## Summary

✅ Clean separation achieved
✅ AuthService handles authentication
✅ DomainUserService handles business logic
✅ Two registration flows (public vs staff)
✅ BranchId required for all staff
✅ SuperAdmin creation blocked via API
✅ Audit log tracks affected branch
✅ All commands explicitly named
✅ Build successful

**Status:** COMPLETE AND READY FOR TESTING 🎉
