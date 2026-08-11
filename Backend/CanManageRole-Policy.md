# CanManageRole Policy - Clear Role Authorization

## ✅ **Why We Created This Policy**

### **Problem with `HasHigherRole`:**
- ❌ Name is confusing: "HasHigherRole" sounds like "I have a higher role"
- ❌ Actually checks: "Can I manage this role?"
- ❌ Not immediately clear what it does

### **Solution: `CanManageRole`:**
- ✅ Name is explicit: "Can I manage users with this role?"
- ✅ Clear intent when reading code
- ✅ Same logic as HasHigherRole, but better naming

---

## 🎯 **What It Does**

**Checks:** "Does the current user have authority to manage users with a specific role?"

**Resource:** Target role (string) - the role we want to check authority over

**Use Cases:**
1. ✅ Role changes (can I change someone with this role?)
2. ✅ User creation (can I create users with this role?)
3. ✅ User deletion (can I delete someone with this role?)
4. ✅ Password reset by admin (can I reset password for someone with this role?)

---

## 📊 **Role Hierarchy**

```
SuperAdmin (highest)
    ├─ Can manage: Director, Administrator, Receptionist, Teacher, User
    └─ Cannot manage: SuperAdmin (themselves)

Director
    ├─ Can manage: Administrator, Receptionist, Teacher, User
    └─ Cannot manage: Director, SuperAdmin

Administrator
    ├─ Can manage: Teacher, User
    └─ Cannot manage: Administrator, Director, SuperAdmin

Receptionist / Teacher / User
    └─ Cannot manage anyone
```

---

## 💡 **Usage Examples**

### **Example 1: Check if user can change a role**

```csharp
// Can Administrator manage a Director?
var canManage = await _authorizationService.AuthorizeAsync(
    User,        // Administrator (from JWT)
    "Director",  // Target role
    "CanManageRole"
);

// Result: Fail (Administrator cannot manage Director)
```

### **Example 2: ChangeRole with both old and new role checks**

```csharp
[HttpPut("{username}/role")]
public async Task<IActionResult> ChangeRole(string userId, ChangeRoleRequestDto request)
{
    var oldRole = await GetUserRole(userId);

    // Check 1: Can I manage user's CURRENT role?
    var oldRoleCheck = await _authorizationService.AuthorizeAsync(
        User, oldRole, "CanManageRole");
    
    if (!oldRoleCheck.Succeeded)
        return Forbid(); // Can't manage this person

    // Check 2: Can I assign the NEW role?
    var newRoleCheck = await _authorizationService.AuthorizeAsync(
        User, request.NewRole, "CanManageRole");
    
    if (!newRoleCheck.Succeeded)
        return Forbid(); // Can't assign this role

    // Change role...
}
```

### **Example 3: CreateStaffUser**

```csharp
[HttpPost("create-staff-user")]
public async Task<IActionResult> CreateStaffUser(CreateStaffUserRequestDto request)
{
    // Can I create users with this role?
    var roleCheck = await _authorizationService.AuthorizeAsync(
        User, request.Role, "CanManageRole");
    
    if (!roleCheck.Succeeded)
        return Forbid(); // Can't create users with this role

    // Create user...
}
```

---

## 🔧 **Handler Implementation**

```csharp
public class CanManageRoleHandler : AuthorizationHandler<CanManageRoleRequirement, string>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        CanManageRoleRequirement requirement, 
        string targetRole) // The role we're checking
    {
        var currentUserRole = context.User.FindFirstValue(ClaimTypes.Role);

        // Define management hierarchy
        var canManage = currentUserRole switch
        {
            "SuperAdmin" => new[] { "Director", "Administrator", "Receptionist", "Teacher", "User" }.Contains(targetRole),
            "Director" => new[] { "Administrator", "Receptionist", "Teacher", "User" }.Contains(targetRole),
            "Administrator" => new[] { "Teacher", "User" }.Contains(targetRole),
            _ => false
        };

        if (canManage)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
```

---

## 🎭 **Attack Prevention**

### **Attack 1: Administrator demotes Director**

```csharp
// Administrator tries to change Director's role to Teacher
var oldRoleCheck = await _authorizationService.AuthorizeAsync(
    User,       // Administrator
    "Director", // Target's current role
    "CanManageRole"
);

// Handler checks:
// Administrator can manage: [Teacher, User]
// "Director" in list? NO
// Result: Fail ❌ ATTACK BLOCKED!
```

### **Attack 2: Director demotes SuperAdmin**

```csharp
// Director tries to change SuperAdmin's role
var oldRoleCheck = await _authorizationService.AuthorizeAsync(
    User,         // Director
    "SuperAdmin", // Target's current role
    "CanManageRole"
);

// Handler checks:
// Director can manage: [Administrator, Receptionist, Teacher, User]
// "SuperAdmin" in list? NO
// Result: Fail ❌ ATTACK BLOCKED!
```

---

## 🆚 **CanManageRole vs HasHigherRole**

| Policy | Name Clarity | Functionality | Use Going Forward |
|--------|--------------|---------------|-------------------|
| **HasHigherRole** | ❌ Confusing | ✅ Works correctly | ⚠️ Keep for backward compatibility |
| **CanManageRole** | ✅ Clear | ✅ Same logic | ✅ Use in new code |

**Recommendation:** 
- Keep both policies (same logic, different names)
- Use `CanManageRole` in new code for clarity
- `HasHigherRole` remains for backward compatibility

---

## 📋 **Where to Use**

✅ **Always use CanManageRole when:**
1. Changing user roles (check both old AND new role)
2. Creating users with specific roles
3. Deleting users (check their role first)
4. Admin password reset (check target user's role)
5. Suspending/banning users

❌ **Don't use for:**
- Self operations (use `SelfOrSuperAdmin` instead)
- Branch isolation (use `IsSameBranch` instead)

---

## ✅ **Summary**

**Policy Name:** `CanManageRole`  
**Purpose:** Check if current user can manage users with a specific role  
**Resource Type:** `string` (role name)  
**Prevents:**
- Lower-level users managing higher-level users
- Privilege escalation attacks
- Unauthorized role assignments

**Usage Pattern:**
```csharp
var canManage = await _authorizationService.AuthorizeAsync(
    User, targetRole, "CanManageRole");
```

**Result:** Clear, explicit role authorization! 🎯
