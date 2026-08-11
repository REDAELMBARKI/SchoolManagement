# AccountController Authorization Summary

## ✅ Completed Authorization Implementation

### **Policies Added:**

#### **1. Role-Based Policies (Simple)**
```csharp
// SuperAdmin only
"IsSuperAdmin" → RequireRole("SuperAdmin")

// Director or SuperAdmin
"IsDirectorOrAbove" → RequireRole("SuperAdmin", "Director")

// Administrator, Director, or SuperAdmin
"IsAdministratorOrAbove" → RequireRole("SuperAdmin", "Director", "Administrator")
```

#### **2. Resource-Based Policies (Complex - with custom handlers)**
```csharp
// User can access their own data OR is SuperAdmin
"SelfOrSuperAdmin" → SelfOrSuperAdminRequirement + SelfOrSuperAdminHandler

// User has higher role than target role (for role changes)
"HasHigherRole" → HasHigherRoleRequirement + HasHigherRoleHandler
```

---

## 📋 AccountController Endpoints Authorization

| # | Endpoint | Auth Applied | Explanation |
|---|----------|--------------|-------------|
| 1 | `POST /register` | `[AllowAnonymous]` | Public registration (students/parents) |
| 2 | `POST /create-staff-user` | `[Authorize(Policy = "IsDirectorOrAbove")]` | SuperAdmin or Director can create staff |
| 3 | `POST /login` | `[AllowAnonymous]` | Public login |
| 4 | `POST /change-password` | `[Authorize]` + `SelfOrSuperAdmin` | User changes own password OR SuperAdmin changes anyone's |
| 5 | `POST /forgot-password` | `[AllowAnonymous]` | Public password reset request |
| 6 | `POST /reset-password` | `[AllowAnonymous]` | Public password reset with token |
| 7 | `POST /confirm-email` | `[AllowAnonymous]` | Public email confirmation with token |
| 8 | `PUT /{username}/role` | `[Authorize]` + `HasHigherRole` | Role change based on hierarchy |
| 9 | `POST /{userId}/claims` | `[Authorize(Policy = "IsSuperAdmin")]` | Only SuperAdmin can add claims |
| 10 | `DELETE /{userId}/claims/{claimType}` | `[Authorize(Policy = "IsSuperAdmin")]` | Only SuperAdmin can remove claims |
| 11 | `GET /{userId}/claims` | `[Authorize]` + `SelfOrSuperAdmin` | User views own claims OR SuperAdmin views anyone's |
| 12 | `GET /{userId}/roles` | `[Authorize]` + `SelfOrSuperAdmin` | User views own roles OR SuperAdmin views anyone's |
| 13 | `GET /user/{id}` | `[Authorize(Policy = "users:view")]` | Requires users:view claim |

---

## 🔧 Implementation Details

### **Resource-Based Authorization Pattern:**

When you need to check if a user can access a specific resource:

```csharp
[Authorize] // Step 1: Verify JWT token (user is authenticated)
public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
{
    // Step 2: Check resource-based policy
    var authResult = await _authorizationService.AuthorizeAsync(
        User,                         // Current logged-in user (from JWT)
        request.ApplicationUserId,    // Resource: target user ID
        "SelfOrSuperAdmin"            // Policy name
    );

    if (!authResult.Succeeded)
    {
        return Forbid(); // 403 Forbidden
    }

    // Step 3: Execute action
    await _authService.ChangePasswordAsync(...);
    return Ok();
}
```

### **SelfOrSuperAdminHandler Logic:**

```csharp
protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context, 
    SelfOrSuperAdminRequirement requirement, 
    string targetApplicationUserId)
{
    var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var currentRole = context.User.FindFirstValue(ClaimTypes.Role);

    // Allow if SuperAdmin
    if (currentRole == "SuperAdmin")
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }

    // Allow if accessing own data (Self)
    if (currentUserId == targetApplicationUserId)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }

    // Deny otherwise
    return Task.CompletedTask;
}
```

### **HasHigherRoleHandler Logic:**

Role hierarchy management:
- **SuperAdmin** can manage: Administrator, Director, Receptionist, Teacher, User
- **Director** can manage: Administrator, Receptionist, Teacher, User
- **Administrator** can manage: Teacher, User

---

## 🎯 Key Concepts

### **"Self" means:**
The logged-in user accessing **their OWN data**.

**Example:**
- User A (userId="123") changes **their own password** ✅ Allowed
- User A (userId="123") changes User B's password (userId="456") ❌ Denied
- SuperAdmin changes anyone's password ✅ Allowed

---

## ⚠️ Important Notes

### **1. Resource-Based vs Role-Based Policies:**

| Type | When to use | Example |
|------|-------------|---------|
| **Role-Based** | Action requires specific role | `[Authorize(Policy = "IsSuperAdmin")]` |
| **Resource-Based** | Action depends on the resource | `_authorizationService.AuthorizeAsync(User, resource, policy)` |

### **2. Cannot use `[Authorize(Policy)]` for Resource-Based:**

❌ **This won't work:**
```csharp
[Authorize(Policy = "SelfOrSuperAdmin")] // No way to pass targetUserId!
public async Task<IActionResult> ChangePassword(...)
```

✅ **Must use manual authorization:**
```csharp
[Authorize] // Just authenticate
public async Task<IActionResult> ChangePassword(...)
{
    var authResult = await _authorizationService.AuthorizeAsync(
        User, 
        request.ApplicationUserId, // Resource
        "SelfOrSuperAdmin"
    );
    if (!authResult.Succeeded) return Forbid();
    // Continue...
}
```

---

## 📦 Files Modified

1. ✅ `SchoolManagement.CrossCutting.Identity/Authorizations/Extensions/AccountsAuthorizations.cs`
   - Added role-based policies
   - Added resource-based policies
   - Registered authorization handlers

2. ✅ `SchoolManagement.CrossCutting.Identity/Authorizations/Requirements/SelfOrSuperAdminRequirement.cs` (NEW)
   - Requirement for Self or SuperAdmin access

3. ✅ `SchoolManagement.CrossCutting.Identity/Authorizations/Handlers/SelfOrSuperAdminHandler.cs` (NEW)
   - Handler logic for Self or SuperAdmin access

4. ✅ `SchoolManagement.Api/Controllers/Auth/AccountController.cs`
   - Applied correct policies to all 13 endpoints
   - Fixed invalid policy syntax
   - Added resource-based authorization checks

---

## 🚀 Next Steps

1. **Test authorization with different roles:**
   - Test as User (can only access own data)
   - Test as Director (can create staff, manage roles)
   - Test as SuperAdmin (full access)

2. **Verify JWT contains correct claims:**
   - `ClaimTypes.NameIdentifier` → ApplicationUserId
   - `ClaimTypes.Role` → User's role

3. **Apply SameBranch policy to domain controllers:**
   - StudentController
   - InvoiceController
   - GroupController
   - etc.

---

## 🎓 SelfOrSuperAdmin vs SameBranch

| Policy | Layer | Resource Type | What it protects |
|--------|-------|---------------|------------------|
| **SelfOrSuperAdmin** | Identity | `ApplicationUserId` (string) | Account credentials (password, claims, roles) |
| **SameBranch** | Domain | Domain entities with `BranchId` | Business data (students, invoices, groups) |

**They are NOT replacements** - they work on different layers for different purposes!
