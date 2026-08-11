# AccountController Edge Cases - Fixed

## ✅ Edge Cases Implemented

### **1. CreateStaffUser - Role Hierarchy Check** ✅

**Problem:** Director could create another Director without validation.

**Fix:** Added `HasHigherRole` authorization check.

```csharp
// Validation 3: Check role hierarchy
var authResult = await _authorizationService.AuthorizeAsync(
    User, 
    request.Role, // Target role being created
    "HasHigherRole"
);

if (!authResult.Succeeded)
{
    return Forbid(); // 403 - You cannot create users with this role
}
```

**Example:**
- ✅ SuperAdmin creates Director (allowed)
- ✅ Director creates Administrator (allowed)
- ❌ Director creates Director (blocked - same level)
- ❌ Administrator creates Director (blocked - higher level)

---

### **2. CreateStaffUser - Branch Isolation (Using IsSameBranchId Policy)** ✅

**Problem:** Director/Administrator could create staff for other branches.

**Fix:** Added `IsSameBranchId` authorization policy (reusable across the app).

```csharp
// Validation 4: Non-SuperAdmin users can only create staff in their own branch
var branchAuthResult = await _authorizationService.AuthorizeAsync(
    User,
    request.BranchId.Value, // Target BranchId
    "IsSameBranchId"
);

if (!branchAuthResult.Succeeded)
{
    return Forbid(); // 403 - You can only create staff in your own branch
}
```

**Handler Logic:**
```csharp
// SuperAdmin bypasses
if (role == "SuperAdmin") 
{
    context.Succeed(requirement);
}

// Check if user's BranchId matches target BranchId
if (userBranchId == targetBranchId)
{
    context.Succeed(requirement);
}
```

**Example:**
- ✅ Director (Branch A) creates staff in Branch A (allowed)
- ❌ Director (Branch A) creates staff in Branch B (blocked)
- ✅ SuperAdmin creates staff in any branch (allowed - bypasses check)

---

### **3. ChangeRole - Prevent Self Role Change** ✅

**Problem:** User could change their own role (privilege escalation).

**Fix:** Added self-check before role change.

```csharp
var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

// Edge Case: User cannot change their own role
if (currentUserId == applicationUserId)
{
    return BadRequest(new { error = "You cannot change your own role." });
}
```

**Example:**
- ❌ Director promotes themselves to SuperAdmin (blocked)
- ❌ Administrator promotes themselves to Director (blocked)
- ✅ SuperAdmin changes another user's role (allowed)

---

## 🎯 New Reusable Policy: IsSameBranchId

### **Policy Details:**
- **Name:** `IsSameBranchId`
- **Requirement:** `SameBranchIdRequirement`
- **Handler:** `SameBranchIdAuthorizationHandler`
- **Resource:** `Guid` (target BranchId)

### **When to use:**
✅ Any action where non-SuperAdmin users should only affect their own branch
✅ Create/Update operations with BranchId parameter
✅ Cross-branch validation

### **Usage Pattern:**
```csharp
var authResult = await _authorizationService.AuthorizeAsync(
    User,
    targetBranchId, // Guid
    "IsSameBranchId"
);

if (!authResult.Succeeded)
{
    return Forbid(); // 403
}
```

### **Difference from IsSameBranch:**
| Policy | Resource Type | Use Case |
|--------|---------------|----------|
| **IsSameBranch** | `Branch` entity | When you have the full entity object |
| **IsSameBranchId** | `Guid` (BranchId) | When you only have the BranchId value |

---

## 📋 Complete Validation Flow

### **CreateStaffUser Validations (in order):**

1. ✅ **Prevent SuperAdmin creation** (business rule)
2. ✅ **Require BranchId** (data integrity)
3. ✅ **Check HasHigherRole** (role hierarchy - NEW!)
4. ✅ **Check IsSameBranchId** (branch security - NEW with policy!)

### **ChangeRole Validations (in order):**

1. ✅ **Prevent self role change** (privilege escalation - NEW!)
2. ✅ **Check HasHigherRole** (role hierarchy)

---

## ⚠️ Remaining Edge Case (Future Implementation)

### **4. AdminResetPassword - SuperAdmin password reset without old password**

**Problem:** Current `ChangePassword` requires old password even for SuperAdmin.

**Current Workaround:** SuperAdmin must use "Forgot Password" flow.

**Recommended Solution:** Create dedicated endpoint `AdminResetPassword`.

```csharp
[HttpPost("{applicationUserId}/admin-reset-password")]
[Authorize(Policy = "IsSuperAdmin")]
public async Task<IActionResult> AdminResetPassword(string applicationUserId, [FromBody] AdminResetPasswordDto request)
{
    // SuperAdmin can reset password without knowing old password
    await _authService.AdminResetPasswordAsync(applicationUserId, request.NewPassword);
    return Ok(new { message = "Password reset by admin successfully." });
}
```

**When to implement:** When SuperAdmin needs to reset locked-out user passwords.

---

## 📊 Complete Validation Flow

### **CreateStaffUser Validations (in order):**

1. ✅ **Prevent SuperAdmin creation** (business rule)
2. ✅ **Require BranchId** (data integrity)
3. ✅ **Check HasHigherRole** (role hierarchy)
4. ✅ **Check branch isolation for Director** (branch security)

### **ChangeRole Validations (in order):**

1. ✅ **Prevent self role change** (privilege escalation)
2. ✅ **Check HasHigherRole** (role hierarchy)

---

## 🎯 Security Summary

| Endpoint | Edge Cases Covered |
|----------|-------------------|
| `POST /create-staff-user` | ✅ Role hierarchy<br>✅ Branch isolation<br>✅ SuperAdmin prevention<br>✅ BranchId required |
| `PUT /{username}/role` | ✅ Self-change prevention<br>✅ Role hierarchy |
| `POST /change-password` | ✅ Self or SuperAdmin only |
| `GET /{userId}/claims` | ✅ Self or SuperAdmin only |
| `GET /{userId}/roles` | ✅ Self or SuperAdmin only |
| `POST /{userId}/claims` | ✅ SuperAdmin only |
| `DELETE /{userId}/claims/{claimType}` | ✅ SuperAdmin only |

---

## 🔒 Role Hierarchy Enforcement

**HasHigherRole Policy:**
- **SuperAdmin** can manage: Director, Administrator, Receptionist, Teacher, User
- **Director** can manage: Administrator, Receptionist, Teacher, User
- **Administrator** can manage: Teacher, User

**Applied to:**
1. ✅ CreateStaffUser (create users with specific role)
2. ✅ ChangeRole (change user's role)

**NOT applied to:**
- Claims management (SuperAdmin only - no hierarchy needed)
- Password changes (Self or SuperAdmin - no hierarchy needed)

---

## 🚀 Testing Scenarios

### **Scenario 1: Director creates staff**
1. Director (Branch A) creates Administrator (Branch A) → ✅ Success
2. Director (Branch A) creates Administrator (Branch B) → ❌ 403 Forbidden (branch mismatch)
3. Director creates Director → ❌ 403 Forbidden (role hierarchy)

### **Scenario 2: Administrator manages roles**
1. Administrator changes Teacher role → ✅ Success
2. Administrator changes Director role → ❌ 403 Forbidden (higher role)
3. Administrator changes own role → ❌ 400 Bad Request (self-change)

### **Scenario 3: SuperAdmin operations**
1. SuperAdmin creates Director (any branch) → ✅ Success
2. SuperAdmin changes any role → ✅ Success
3. SuperAdmin changes own role → ❌ 400 Bad Request (self-change - even SuperAdmin cannot)

---

## 📝 Notes

1. **BranchId claim:** Ensure JWT contains `BranchId` claim for Directors (required for validation 4).
2. **SuperAdmin BranchId:** SuperAdmin has `BranchId = null` or Guid.Empty - check bypasses this.
3. **Self-change:** Even SuperAdmin cannot change their own role (prevents accidents).

