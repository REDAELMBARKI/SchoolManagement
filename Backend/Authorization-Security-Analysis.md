# Authorization Security Analysis - Vulnerabilities & Edge Cases

## 🔴 **CRITICAL VULNERABILITIES FOUND**

---

### **1. ⚠️ CRITICAL: ChangeRole Missing Old Role Check**

**Location:** `AccountController.ChangeRole()`

**Problem:**
```csharp
public async Task<IActionResult> ChangeRole(string applicationUserId, ...)
{
    var oldRoles = await _authService.GetUserRolesAsync(applicationUserId);
    var oldRole = oldRoles.FirstOrDefault() ?? string.Empty;

    // Only checks if user can assign NEW role
    var authorizationResult = await _authorizationService.AuthorizeAsync(
        User, request.NewRole, "HasHigherRole");
    
    // ❌ MISSING: Check if user has higher role than CURRENT role
}
```

**Attack Scenario:**
- Administrator demotes a Director to Teacher
- Administrator only has authority over Teacher/User
- Director is HIGHER than Administrator
- **Result:** ❌ Administrator can demote someone higher than them!

**Impact:** **HIGH** - Role hierarchy bypass

**Fix:**
```csharp
// Check 1: Can manage the OLD role (target user's current role)
var oldRoleCheck = await _authorizationService.AuthorizeAsync(
    User, oldRole, "HasHigherRole");

if (!oldRoleCheck.Succeeded)
{
    return Forbid(); // You cannot change roles of users higher than you
}

// Check 2: Can assign the NEW role
var newRoleCheck = await _authorizationService.AuthorizeAsync(
    User, request.NewRole, "HasHigherRole");

if (!newRoleCheck.Succeeded)
{
    return Forbid(); // You cannot assign this role
}
```

---

### **2. ⚠️ HIGH: No Branch Check in ChangeRole**

**Location:** `AccountController.ChangeRole()`

**Problem:**
- Director from Branch A can change roles of users in Branch B
- No `IsSameBranch` check applied

**Attack Scenario:**
- Director (Branch A) changes role of Administrator (Branch B)
- **Result:** ❌ Cross-branch role manipulation

**Impact:** **HIGH** - Branch isolation bypass

**Fix:**
```csharp
// Get target user's DomainUser to check branch
var targetDomainUser = await _domainUserService.GetByApplicationUserIdAsync(applicationUserId);

// Check if same branch (SuperAdmin bypasses)
var branchCheck = await _authorizationService.AuthorizeAsync(
    User, targetDomainUser.BranchId, "IsSameBranch");

if (!branchCheck.Succeeded)
{
    return Forbid(); // You can only change roles in your own branch
}
```

---

### **3. ⚠️ HIGH: AddClaim/RemoveClaim No Branch Isolation**

**Location:** `AccountController.AddClaim()`, `RemoveClaim()`

**Problem:**
- Only `IsSuperAdmin` policy
- No check if target user is in same branch
- Director could add claims to users in other branches if they somehow get access

**Current:**
```csharp
[Authorize(Policy = "IsSuperAdmin")] // Only role check
public async Task<IActionResult> AddClaim(string applicationUserId, ...)
```

**Impact:** **MEDIUM** - Currently mitigated by IsSuperAdmin, but not defense-in-depth

**Recommendation:** Keep SuperAdmin-only (claims are system-level, not branch-level)

---

### **4. ⚠️ MEDIUM: No Validation of Role String in HasHigherRole**

**Location:** `HasHigherRoleHandler`

**Problem:**
```csharp
var superAdminManagedRole = new [] { 
    RoleHelper.Administrator, 
    RoleHelper.Director, 
    RoleHelper.Reciptionest, // ❌ TYPO!
    RoleHelper.Teacher,
    RoleHelper.User 
};

// What if request.Role = "HackerRole"?
// Result: Not in array → Fail (GOOD)
// But no explicit error message
```

**Attack Scenario:**
- Attacker sends `request.Role = "AdminXXX"`
- Policy fails silently (403 Forbidden)
- No audit log of attempted invalid role

**Impact:** **LOW** - Fails safely, but no audit trail

**Fix:** Add role validation before authorization check:
```csharp
var validRoles = new[] { "SuperAdmin", "Director", "Administrator", "Receptionist", "Teacher", "User" };

if (!validRoles.Contains(request.Role))
{
    return BadRequest(new { error = $"Invalid role: {request.Role}" });
}
```

---

### **5. ⚠️ MEDIUM: SelfOrSuperAdmin Null Checks Missing**

**Location:** `SelfOrSuperAdminHandler`

**Problem:**
```csharp
var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
var currentRole = context.User.FindFirstValue(ClaimTypes.Role);

// ❌ What if currentUserId is null?
if (currentUserId == targetApplicationUserId) // null == "some-id" → false
{
    context.Succeed(requirement);
}
```

**Attack Scenario:**
- Malformed JWT without NameIdentifier claim
- Handler doesn't fail explicitly
- Just doesn't succeed (correct behavior, but unclear)

**Impact:** **LOW** - Fails safely

**Fix:** Add explicit null checks:
```csharp
if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(currentRole))
{
    // Explicitly fail for missing claims
    return Task.CompletedTask;
}
```

---

### **6. ⚠️ LOW: Missing Rate Limiting on Password Operations**

**Location:** `ChangePassword`, `ForgotPassword`, `ResetPassword`

**Problem:**
- No rate limiting on password change attempts
- Brute force password reset tokens
- Account enumeration via forgot-password

**Attack Scenario:**
- Attacker calls `/forgot-password` with many emails
- Server responds differently for existing vs non-existing users
- **Result:** Email enumeration attack

**Impact:** **LOW** - Common issue, needs rate limiting middleware

**Fix:** 
1. Add rate limiting (e.g., 5 attempts per 15 minutes)
2. Return same response for existing and non-existing emails

---

### **7. ⚠️ LOW: Typo in Role Name**

**Location:** `HasHigherRoleHandler`

```csharp
RoleHelper.Reciptionest // ❌ Should be "Receptionist"
```

**Impact:** **LOW** - If this is the actual role name in DB, fine. Otherwise, Receptionist role won't work correctly.

**Fix:** Check RoleHelper for correct spelling

---

## 🟡 **POTENTIAL ISSUES**

### **8. Missing Branch Check in GetUserById**

**Location:** `AccountController.GetUserById()`

**Current:**
```csharp
[Authorize(Policy = "IsAdministratorOrAbove")]
public async Task<IActionResult> GetUserById(Guid id)
{
    var user = await _domainUserService.GetByIdAsync(id);
    return Ok(user);
}
```

**Problem:**
- Administrator from Branch A can view DomainUser from Branch B
- No branch isolation

**Impact:** **MEDIUM** - Information disclosure across branches

**Fix:**
```csharp
var user = await _domainUserService.GetByIdAsync(id);

// Check if same branch
var branchCheck = await _authorizationService.AuthorizeAsync(
    User, user.BranchId, "IsSameBranch");

if (!branchCheck.Succeeded)
{
    return Forbid();
}

return Ok(user);
```

---

### **9. No Audit Logging**

**Problem:**
- No audit trail for:
  - Role changes
  - Claim modifications
  - Staff user creation
  - Password changes by SuperAdmin

**Impact:** **MEDIUM** - No forensics capability

**Recommendation:** Add audit logging for all sensitive operations

---

### **10. JWT Token Refresh Not Mentioned**

**Problem:**
- Role/claim changes require user re-login
- No token refresh mechanism shown
- User session persists with old roles until token expires

**Impact:** **LOW** - Known limitation, but should be documented

---

## 📊 **SEVERITY SUMMARY**

| Severity | Count | Issues |
|----------|-------|--------|
| 🔴 **CRITICAL** | 2 | ChangeRole missing old role check, ChangeRole no branch check |
| 🟠 **HIGH** | 1 | AddClaim/RemoveClaim no branch isolation (mitigated by SuperAdmin-only) |
| 🟡 **MEDIUM** | 3 | No role validation, SelfOrSuperAdmin null checks, GetUserById no branch check |
| 🟢 **LOW** | 3 | Rate limiting, typo, audit logging |

---

## ✅ **IMMEDIATE FIXES REQUIRED**

### **Priority 1: Fix ChangeRole**
1. Add old role authorization check
2. Add branch isolation check
3. Add role string validation

### **Priority 2: Fix GetUserById**
1. Add branch isolation check

### **Priority 3: Add Role Validation**
1. Validate role strings before authorization

---

## 🎯 **DEFENSE-IN-DEPTH LAYERS**

Current layers:
1. ✅ JWT authentication
2. ✅ Role-based policies
3. ✅ Resource-based policies (HasHigherRole, IsSameBranch, SelfOrSuperAdmin)
4. ✅ EF Global Query Filters (branch isolation at DB level)
5. ❌ Missing: Input validation (role strings)
6. ❌ Missing: Audit logging
7. ❌ Missing: Rate limiting

---

## 📋 **TESTING SCENARIOS**

### **Test 1: ChangeRole Bypass**
1. Create Director (Branch A)
2. Create Administrator (Branch A)
3. Login as Administrator
4. Try to change Director's role to Teacher
5. **Expected:** ❌ 403 Forbidden (Administrator cannot manage Director)
6. **Current:** ✅ Might succeed if only checking NEW role!

### **Test 2: Cross-Branch Role Change**
1. Create Director (Branch A)
2. Create Administrator (Branch B)
3. Login as Director (Branch A)
4. Try to change Administrator (Branch B) role
5. **Expected:** ❌ 403 Forbidden (different branch)
6. **Current:** ❌ Might succeed (no branch check)!

---

**RECOMMENDATION: Fix Priority 1 issues immediately before production deployment.** 🔒
