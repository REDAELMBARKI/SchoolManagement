# Authorization Approaches for Branch Access Control

## Problem
You want to enforce branch-level access control globally:
- SuperAdmin can access ALL branches
- All other users can ONLY access their assigned branch
- This should apply to (almost) every request

---

## ❌ What You Had (BROKEN)

### Issues in Your Code:

```csharp
// ❌ WRONG
if(resource is null)
{
    AuthorizationResult.Success(); // Creates object, doesn't succeed!
}

if(context.User.FindFirstValue("BranchId") == resource!.Id.ToString())
{
    AuthorizationResult.Success(); // Same issue
}

return Task.CompletedTask; // ALWAYS returns without succeeding = ALWAYS FAILS
```

**Problems:**
1. ❌ `AuthorizationResult.Success()` just creates an object - you need `context.Succeed(requirement)`
2. ❌ No SuperAdmin bypass
3. ❌ Resource-based handler requires a `Branch` object - can't apply globally
4. ❌ Handler not registered in DI

---

## ✅ Solution 1: Authorization Filter (RECOMMENDED for Global)

### When to Use:
- ✅ Need to apply check to EVERY request
- ✅ Check doesn't need a specific resource
- ✅ Want to skip based on role (SuperAdmin)
- ✅ Want to store branch in HttpContext for controllers

### Implementation:

```csharp
// BranchAccessFilter.cs
public class BranchAccessFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Allow anonymous
        if (!user.Identity?.IsAuthenticated ?? true)
            return;

        var role = user.FindFirstValue(ClaimTypes.Role);

        // SuperAdmin bypasses
        if (role == "SuperAdmin")
            return;

        var userBranchId = user.FindFirstValue("BranchId");
        
        // Non-SuperAdmin MUST have BranchId
        if (string.IsNullOrEmpty(userBranchId))
        {
            context.Result = new ForbidResult();
            return;
        }

        // Store for controllers
        context.HttpContext.Items["UserBranchId"] = userBranchId;
    }
}
```

### Register in Program.cs:

```csharp
builder.Services.AddControllers(options =>
{
    // Apply globally to ALL controllers
    options.Filters.Add<BranchAccessFilter>();
});
```

### Usage in Controllers:

```csharp
[ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Filter automatically applied
        // SuperAdmin sees all, others see their branch only
        
        // Get user's branch from HttpContext
        var userBranchId = HttpContext.Items["UserBranchId"]?.ToString();
        
        // Your service layer can use this
        var students = await _service.GetByBranchAsync(userBranchId);
        return Ok(students);
    }

    [AllowAnonymous] // Skip filter for public endpoints
    [HttpGet("public")]
    public IActionResult GetPublicData()
    {
        return Ok("Public data");
    }
}
```

---

## ✅ Solution 2: Resource-Based Policy (For Specific Use Cases)

### When to Use:
- ✅ Need to check access to a SPECIFIC resource
- ✅ Have the resource object available
- ✅ Want to use `[Authorize(Policy = "IsSameBranch")]`

### Fixed Handler:

```csharp
public class SameBranchAuthorizationHandler : AuthorizationHandler<SameBranchRequirement, Branch>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        SameBranchRequirement requirement, 
        Branch resource)
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);

        // SuperAdmin bypasses
        if (role == "SuperAdmin")
        {
            context.Succeed(requirement); // ✅ CORRECT
            return Task.CompletedTask;
        }

        // Resource required
        if (resource is null)
            return Task.CompletedTask;

        var userBranchId = context.User.FindFirstValue("BranchId");

        if (string.IsNullOrEmpty(userBranchId))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // Check match
        if (userBranchId == resource.Id.ToString())
        {
            context.Succeed(requirement); // ✅ CORRECT
        }

        return Task.CompletedTask;
    }
}
```

### Register in DI:

```csharp
public static IServiceCollection AddBranchingAuthorizations(this IServiceCollection services)
{
    // Register handler
    services.AddScoped<IAuthorizationHandler, SameBranchAuthorizationHandler>();

    services.AddAuthorization(options =>
    {
        options.AddPolicy("IsSameBranch", policy =>
        { 
            policy.AddRequirements(new SameBranchRequirement());
        });
    });
    
    return services;
}
```

### Usage in Controllers:

```csharp
[ApiController]
[Route("api/branches")]
public class BranchController : ControllerBase
{
    private readonly IAuthorizationService _authService;
    private readonly IBranchService _branchService;

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateBranchDto dto)
    {
        var branch = await _branchService.GetByIdAsync(id);
        
        // Manually invoke policy with resource
        var authResult = await _authService.AuthorizeAsync(
            User, 
            branch,  // ← Pass the resource
            "IsSameBranch"
        );

        if (!authResult.Succeeded)
            return Forbid();

        // Proceed with update
        await _branchService.UpdateAsync(id, dto);
        return Ok();
    }
}
```

---

## 📊 Comparison

| Feature | Authorization Filter | Resource-Based Policy |
|---------|---------------------|----------------------|
| **Global Application** | ✅ Easy (one line in Program.cs) | ❌ Can't apply globally |
| **SuperAdmin Bypass** | ✅ Built-in | ✅ Built-in (fixed) |
| **Needs Resource Object** | ❌ No | ✅ Yes (Branch object) |
| **HttpContext Storage** | ✅ Can store UserBranchId | ❌ No |
| **Attribute Usage** | ❌ No attribute | ✅ `[Authorize(Policy="IsSameBranch")]` |
| **Manual Invocation** | ❌ No | ✅ `AuthorizeAsync(User, resource, "IsSameBranch")` |
| **Complexity** | Low | Medium |

---

## 🎯 Recommendation

### Use **Authorization Filter** for:
- ✅ **Global branch filtering** (your main use case)
- Automatically applies to all controllers
- SuperAdmin bypass built-in
- Simple and clean

### Use **Resource-Based Policy** for:
- Specific endpoints where you have the Branch object
- Fine-grained control per action
- When you need to check against actual resource

### Hybrid Approach (BEST):
```csharp
// Global filter for basic branch check
builder.Services.AddControllers(options =>
{
    options.Filters.Add<BranchAccessFilter>();
});

// + Resource-based policy for specific cases
builder.Services.AddBranchingAuthorizations();
```

---

## 🚀 Next Steps

1. ✅ Choose your approach (Filter recommended)
2. ✅ Register in Program.cs
3. ✅ Test with SuperAdmin (should access all)
4. ✅ Test with Director (should access only their branch)
5. ✅ Add `[AllowAnonymous]` to public endpoints
6. ✅ Update services to use UserBranchId from HttpContext

---

## ⚠️ Critical Fixes Applied

1. ✅ Fixed `context.Succeed(requirement)` instead of `AuthorizationResult.Success()`
2. ✅ Added SuperAdmin bypass logic
3. ✅ Registered handler in DI
4. ✅ Created global filter option
5. ✅ Added proper documentation

Your original code would have **NEVER succeeded** because you weren't calling `context.Succeed()`.
