using Microsoft.AspNetCore.Authorization;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
using SchoolManagement.Domain.Common.Utils;
using System.Security.Claims;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Handlers;

/// <summary>
/// Handles authorization for branch-level access using BranchId directly.
/// Ensures users (except SuperAdmin) can only create/modify resources in their own branch.
/// Resource: Target BranchId (Guid)
/// </summary>
public class SameBranchAuthorizationHandler : AuthorizationHandler<SameBranchRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameBranchRequirement requirement,
        Guid targetBranchId)
    {
        // Get user's role
        var role = context.User.FindFirstValue(ClaimTypes.Role);

        // SuperAdmin bypasses branch check
        if (role == RoleHelper.SuperAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Get user's BranchId from claims
        var userBranchIdClaim = context.User.FindFirstValue("BranchId");

        // If user has no BranchId claim, deny
        if (string.IsNullOrEmpty(userBranchIdClaim))
        {
            // Don't fail explicitly - just don't succeed
            return Task.CompletedTask;
        }

        // Parse user's BranchId
        if (!Guid.TryParse(userBranchIdClaim, out var userBranchId))
        {
            // Invalid BranchId format - deny
            return Task.CompletedTask;
        }

        // Check if user's branch matches target branch
        if (userBranchId == targetBranchId)
        {
            context.Succeed(requirement);
        }

        // If branches don't match, don't succeed (implicitly denies)
        return Task.CompletedTask;
    }
}
