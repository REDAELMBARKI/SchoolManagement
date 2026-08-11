using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace SchoolManagement.CrossCutting.Identity.Filters;

/// <summary>
/// Global filter that enforces branch-level access control.
/// SuperAdmin bypasses this check.
/// All other users must have a valid BranchId.
/// 
/// IMPORTANT: This filter only validates that the user HAS a BranchId.
/// Individual resource access (checking if resource.BranchId == user.BranchId)
/// must be done in services/repositories by filtering queries.
/// </summary>
public class BranchAccessFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Allow anonymous endpoints
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return;
        }

        // Get user's role
        var role = user.FindFirstValue(ClaimTypes.Role);

        // SuperAdmin bypasses branch check (no BranchId required)
        if (role == "SuperAdmin")
        {
            context.HttpContext.Items["IsSuperAdmin"] = true;
            return;
        }

        // Get user's BranchId from claims
        var userBranchIdClaim = user.FindFirstValue("BranchId");
        
        // Non-SuperAdmin MUST have a BranchId
        if (string.IsNullOrEmpty(userBranchIdClaim))
        {
            context.Result = new ForbidResult();
            return;
        }

        // Store BranchId in HttpContext for services to use
        context.HttpContext.Items["UserBranchId"] = userBranchIdClaim;
        context.HttpContext.Items["IsSuperAdmin"] = false;

        await Task.CompletedTask;
    }
}
