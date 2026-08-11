# IsSameBranch Policy - Usage Guide

## ✅ Simplified Branch Authorization

**Policy Name:** `IsSameBranch`  
**Requirement:** `SameBranchRequirement`  
**Handler:** `SameBranchAuthorizationHandler`  
**Resource Type:** `Guid` (BranchId - no entity navigation needed!)

---

## 🎯 Purpose

Ensures that non-SuperAdmin users can only create/modify resources in **their own branch**.
- ✅ **SuperAdmin bypasses** this check (can access any branch)
- ✅ **All other roles** must match BranchId

---

## 📝 Usage Pattern

```csharp
// Manual authorization check with resource (BranchId as Guid)
var authResult = await _authorizationService.AuthorizeAsync(
    User,
    targetBranchId, // Guid - the BranchId you're trying to access
    "IsSameBranch"
);

if (!authResult.Succeeded)
{
    return Forbid(); // 403 Forbidden
}
```

---

## 🔧 Handler Logic

```csharp
public class SameBranchAuthorizationHandler : AuthorizationHandler<SameBranchRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameBranchRequirement requirement,
        Guid targetBranchId) // Takes Guid directly!
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);

        // SuperAdmin bypasses
        if (role == "SuperAdmin")
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Get user's BranchId from JWT claims
        var userBranchIdClaim = context.User.FindFirstValue("BranchId");
        if (!Guid.TryParse(userBranchIdClaim, out var userBranchId))
        {
            return Task.CompletedTask; // Deny if invalid
        }

        // Check if branches match
        if (userBranchId == targetBranchId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

---

## 📋 Where It's Used

### **1. AccountController - CreateStaffUser**
```csharp
[HttpPost("create-staff-user")]
public async Task<IActionResult> CreateStaffUser([FromBody] RegisterUserRequestDto request)
{
    // Validation 4: Branch isolation
    var branchAuthResult = await _authorizationService.AuthorizeAsync(
        User,
        request.BranchId.Value, // Guid
        "IsSameBranch"
    );

    if (!branchAuthResult.Succeeded)
    {
        return Forbid(); // 403 - You can only create staff in your own branch
    }

    // Continue...
}
```

### **2. Future Usage - Any Domain Controller**
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentCommand command)
{
    var student = await _studentService.GetByIdAsync(id);
    
    // Check if user can modify student in this branch
    var authResult = await _authorizationService.AuthorizeAsync(
        User,
        student.BranchId, // Guid
        "IsSameBranch"
    );

    if (!authResult.Succeeded) return Forbid();

    // Update...
}
```

---

## 🎭 Examples

### **Example 1: Director creates staff in own branch**
- **User:** Director (BranchId = "aaa-111")
- **Action:** Create staff with BranchId = "aaa-111"
- **Result:** ✅ **Allowed** (same branch)

### **Example 2: Director creates staff in other branch**
- **User:** Director (BranchId = "aaa-111")
- **Action:** Create staff with BranchId = "bbb-222"
- **Result:** ❌ **403 Forbidden** (different branch)

### **Example 3: SuperAdmin creates staff anywhere**
- **User:** SuperAdmin (BranchId = null or Guid.Empty)
- **Action:** Create staff with BranchId = "bbb-222"
- **Result:** ✅ **Allowed** (SuperAdmin bypasses)

### **Example 4: User with no BranchId claim**
- **User:** User (no BranchId claim)
- **Action:** Create staff with BranchId = "aaa-111"
- **Result:** ❌ **403 Forbidden** (no branch claim)

---

## ⚙️ Configuration

### **Policy Registration (already done):**
```csharp
// BranchingAuthorizations.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("IsSameBranch", policy =>
    {
        policy.AddRequirements(new SameBranchRequirement());
    });
});

services.AddScoped<IAuthorizationHandler, SameBranchAuthorizationHandler>();
```

### **JWT Claims Required:**
- ✅ `ClaimTypes.Role` → User's role (e.g., "Director", "SuperAdmin")
- ✅ `"BranchId"` → User's branch ID (Guid as string)

**Example JWT payload:**
```json
{
  "sub": "user-id-123",
  "role": "Director",
  "BranchId": "aaa-111-bbb-222-ccc-333",
  "email": "director@school.com"
}
```

---

## 🔄 Difference from Old Implementation

### **❌ Old (Entity-based):**
```csharp
AuthorizationHandler<SameBranchRequirement, Branch>
// Required navigating to Branch entity first
```

### **✅ New (Guid-based):**
```csharp
AuthorizationHandler<SameBranchRequirement, Guid>
// Works directly with BranchId - no entity needed!
```

**Benefits:**
- ✅ No database query for Branch entity
- ✅ Works with just BranchId value
- ✅ Faster authorization checks
- ✅ Can be used in Create operations (before entity exists)

---

## 🚀 When to Use This Policy

✅ **Use IsSameBranch when:**
- Creating resources with BranchId
- Updating resources in specific branch
- Deleting resources in specific branch
- Any cross-branch operation validation

❌ **Don't use when:**
- Action already filtered by EF Global Query Filters (automatic)
- SuperAdmin-only endpoints (no branch check needed)
- Public endpoints (no user context)

---

## 📊 Summary

| Feature | Value |
|---------|-------|
| **Policy Name** | IsSameBranch |
| **Resource Type** | Guid (BranchId) |
| **SuperAdmin Bypass** | Yes |
| **JWT Claims Required** | Role, BranchId |
| **Use Case** | Branch isolation for all non-SuperAdmin roles |
| **Performance** | Fast (no DB query) |

---

**This policy is now the standard way to enforce branch isolation across the entire application!** 🎯
