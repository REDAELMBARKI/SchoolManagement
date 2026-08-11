using Microsoft.AspNetCore.Authorization;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
using SchoolManagement.Domain.Common.Utils;
using System.Security.Claims;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Handlers
{
    /// <summary>
    /// Allows user to access their own data OR allows SuperAdmin to access any user's data
    /// Resource: target ApplicationUserId (string)
    /// </summary>
    public class SelfOrSuperAdminHandler : AuthorizationHandler<SelfOrSuperAdminRequirement, string>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            SelfOrSuperAdminRequirement requirement, 
            string targetApplicationUserId)
        {
            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentRole = context.User.FindFirstValue(ClaimTypes.Role);

            // Allow if SuperAdmin
            if (currentRole == RoleHelper.SuperAdmin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Allow if accessing own data
            if (currentUserId == targetApplicationUserId)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Deny otherwise
            return Task.CompletedTask;
        }
    }
}
