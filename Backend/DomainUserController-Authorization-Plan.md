# DomainUserController - Authorization Plan

## 📋 **Current Endpoints (10 total):**

| # | Endpoint | Current Auth | Needed Policies |
|---|----------|--------------|-----------------|
| 1 | `GET /api/domain-users` | ❌ None | Role + Branch |
| 2 | `GET /api/domain-users/{id}` | ❌ None | Role + Branch |
| 3 | `PUT /api/domain-users/{id}` | ❌ None | Role + Branch + CanManageRole |
| 4 | `DELETE /api/domain-users/{id}` | ❌ None | Role + Branch + CanManageRole |
| 5 | `POST /{id}/assign-branch` | ❌ None | SuperAdmin only |
| 6 | `POST /{id}/remove-branch` | ❌ None | SuperAdmin only |
| 7 | `POST /{id}/activate` | ❌ None | Role + Branch + CanManageRole |
| 8 | `POST /{id}/deactivate` | ❌ None | Role + Branch + CanManageRole |
| 9 | `GET /branch/{branchId}` | ❌ None | Role + IsSameBranch |
| 10 | `GET /role/{role}` | ❌ None | Role + Branch filtering |

---

## 🎯 **Authorization Strategy:**

### **Layer 1: Role-Based Access (Who can access?)**
- ✅ **IsAdministratorOrAbove** - Administrator, Director, SuperAdmin

### **Layer 2: Branch Isolation (What data can they see?)**
- ✅ **IsSameBranch** - Check target user's branch matches logged-in user's branch
- ✅ SuperAdmin bypasses this automatically

### **Layer 3: Role Hierarchy (Can they manage this user?)**
- ✅ **CanManageRole** - Check if you can manage users with target user's role

---

## 📝 **Detailed Authorization for Each Endpoint:**

### **1. GET /api/domain-users (Get All)**

**Policies:**
```csharp
[Authorize(Policy = "IsAdministratorOrAbove")]
```

**Logic:**
- EF Global Query Filters automatically filter by branch
- SuperAdmin sees all branches (with `.IgnoreQueryFilters()` if needed)
- Director/Administrator only see their own branch (automatic)

**No additional checks needed** - EF handles it!

---

### **2. GET /api/domain-users/{id} (Get By ID)**

**Policies:**
```csharp
[Authorize(Policy = "IsAdministratorOrAbove")]
```

**Manual Checks:**
```csharp
var user = await _service.GetByIdAsync(id);

// Check: Can I view users in this branch?
var branchCheck = await _authorizationService.AuthorizeAsync(
    User, user.BranchId, "IsSameBranch");

if (!branchCheck.Succeeded)
    return Forbid();
```

---

### **3. PUT /api/domain-users/{id} (Update)**

**Policies:**
```csharp
[Authorize(Policy = "IsAdministratorOrAbove")]
```

**Manual Checks:**
```csharp
var user = await _service.GetByIdAsync(id);

// Check 1: Branch isolation
var branchCheck = await _authorizationService.AuthorizeAsync(
    User, user.BranchId, "IsSameBranch");

if (!branchCheck.Succeeded)
    return Forbid();

// Check 2: Role hierarchy (can I manage this user's role?)
var roleCheck = await _authorizationService.AuthorizeAsync(
    User, user.Role, "CanManageRole");

if (!roleCheck.Succeeded)
    return Forbid();
```

**Why both checks?**
- Branch check: Prevents cross-branch updates
- Role check: Administrator can't update Director's profile

---

### **4. DELETE /api/domain-users/{id} (Delete)**

**Policies:**
```csharp
[Authorize(Policy = "IsDirectorOrAbove")] // Only Director+ can delete
```

**Manual Checks:**
```csharp
var user = await _service.GetByIdAsync(id);

// Check 1: Branch isolation
var branchCheck = await _authorizationService.AuthorizeAsync(
    User, user.BranchId, "IsSameBranch");

if (!branchCheck.Succeeded)
    return Forbid();

// Check 2: Role hierarchy
var roleCheck = await _authorizationService.AuthorizeAsync(
    User, user.Role, "CanManageRole");

if (!roleCheck.Succeeded)
    return Forbid();
```

---

### **5. POST /{id}/assign-branch (Assign Branch)**

**Policies:**
```csharp
[Authorize(Policy = "IsSuperAdmin")] // Only SuperAdmin
```

**Why SuperAdmin only?**
- Moving users between branches is a critical operation
- Only SuperAdmin has cross-branch authority

**No additional checks needed** - policy is enough

---

### **6. POST /{id}/remove-branch (Remove Branch)**

**Policies:**
```csharp
[Authorize(Policy = "IsSuperAdmin")] // Only SuperAdmin
```

**Same reason as AssignBranch**

---

### **7. POST /{id}/activate (Activate User)**

**Policies:**
```csharp
[Authorize(Policy = "IsAdministratorOrAbove")]
```

**Manual Checks:**
```csharp
var user = await _service.GetByIdAsync(id);

// Check 1: Branch isolation
var branchCheck = await _authorizationService.AuthorizeAsync(
    User, user.BranchId, "IsSameBranch");

if (!branchCheck.Succeeded)
    return Forbid();

// Check 2: Role hierarchy
var roleCheck = await _authorizationService.AuthorizeAsync(
    User, user.Role, "CanManageRole");

if (!roleCheck.Succeeded)
    return Forbid();
```

---

### **8. POST /{id}/deactivate (Deactivate User)**

**Policies:**
```csharp
[Authorize(Policy = "IsAdministratorOrAbove")]
```

**Manual Checks:** Same as Activate

---

### **9. GET /branch/{branchId} (Get Users by Branch)**

**Policies:**
```csharp
[Authorize(Policy = "IsAdministratorOrAbove")]
```

**Manual Checks:**
```csharp
// Check: Can I access this branch?
var branchCheck = await _authorizationService.AuthorizeAsync(
    User, branchId, "IsSameBranch");

if (!branchCheck.Succeeded)
    return Forbid();

// Then query users in that branch
```

**Why?** Prevents Director from querying other branches' users

---

### **10. GET /role/{role} (Get Users by Role)**

**Policies:**
```csharp
[Authorize(Policy = "IsAdministratorOrAbove")]
```

**Logic:**
- EF Global Query Filters automatically filter by branch
- Returns users with specified role **in logged-in user's branch**
- SuperAdmin sees all branches

**No additional checks needed** - EF handles branch filtering!

---

## 📊 **Summary of Policies Needed:**

| Policy | Usage Count | Where Used |
|--------|-------------|------------|
| **IsAdministratorOrAbove** | 8 endpoints | Most read/write operations |
| **IsDirectorOrAbove** | 1 endpoint | Delete only |
| **IsSuperAdmin** | 2 endpoints | Branch assignment operations |
| **IsSameBranch** | 6 endpoints | Manual branch checks |
| **CanManageRole** | 4 endpoints | Update, Delete, Activate, Deactivate |

---

## ✅ **Reusable Policies (Already Exist):**

1. ✅ `IsAdministratorOrAbove` - Already exists
2. ✅ `IsDirectorOrAbove` - Already exists
3. ✅ `IsSuperAdmin` - Already exists
4. ✅ `IsSameBranch` - Already exists
5. ✅ `CanManageRole` - Already exists

**No new policies needed!** ✅

---

## 🔒 **Security Layers:**

### **Defense in Depth:**

1. **JWT Authentication** - User must be logged in
2. **Role-Based Policy** - User must have minimum role
3. **EF Global Query Filters** - Automatic branch filtering on queries
4. **Manual Branch Check** - Explicit check for specific operations
5. **Role Hierarchy Check** - Can't manage higher-level users

**5 layers of protection!** 🛡️

---

## 🚀 **Implementation Priority:**

### **Critical (Do First):**
1. ✅ GetById - branch check
2. ✅ Update - branch + role checks
3. ✅ Delete - branch + role checks

### **High Priority:**
4. ✅ GetByBranch - branch check
5. ✅ Activate/Deactivate - branch + role checks

### **Medium Priority:**
6. ✅ AssignBranch/RemoveBranch - SuperAdmin only
7. ✅ GetAll - role-based + EF filtering
8. ✅ GetByRole - role-based + EF filtering

---

## 🎯 **Next Steps:**

1. Apply `[Authorize]` attributes to all endpoints
2. Add manual authorization checks where needed
3. Test with different roles and branches
4. Verify EF Global Query Filters work correctly

**Ready to implement?**
