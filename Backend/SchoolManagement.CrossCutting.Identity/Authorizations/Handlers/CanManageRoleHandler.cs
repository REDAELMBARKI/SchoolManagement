using Microsoft.AspNetCore.Authorization;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
using SchoolManagement.Domain.Common.Utils;
using System.Security.Claims;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Handlers
{
    /// <summary>
    /// Checks if the current user has authority to manage users with a specific role.
    /// This is used for role changes, user creation, and user deletion.
    /// 
    /// Role Hierarchy:
    /// - SuperAdmin can manage: Director, Administrator, Receptionist, Teacher, User
    /// - Director can manage: Administrator, Receptionist, Teacher, User
    /// - Administrator can manage: Teacher, User
    /// - Others: Cannot manage any roles
    /// </summary>
    public class CanManageRoleHandler : AuthorizationHandler<CanManageRoleRequirement, string>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            CanManageRoleRequirement requirement, 
            string targetRole)
        {
            var currentUserRole = context.User.FindFirstValue(ClaimTypes.Role);

            // Define what roles each level can manage
            var superAdminManagedRoles = new[] 
            { 
                RoleHelper.Administrator, 
                RoleHelper.Director, 
                RoleHelper.Reciptionest, 
                RoleHelper.Teacher, 
                RoleHelper.User 
            };

            var directorManagedRoles = new[] 
            { 
                RoleHelper.Administrator, 
                RoleHelper.Reciptionest, 
                RoleHelper.Teacher, 
                RoleHelper.User 
            };

            var administratorManagedRoles = new[] 
            { 
                RoleHelper.Teacher, 
                RoleHelper.User 
            };

            // Check if current user can manage the target role
            bool canManage = false;

            if (currentUserRole == RoleHelper.SuperAdmin)
            {
                canManage = superAdminManagedRoles.Contains(targetRole);
            }
            else if (currentUserRole == RoleHelper.Director)
            {
                canManage = directorManagedRoles.Contains(targetRole);
            }
            else if (currentUserRole == RoleHelper.Administrator)
            {
                canManage = administratorManagedRoles.Contains(targetRole);
            }
            // All other roles cannot manage anyone (canManage stays false)

            if (canManage)
            {
                context.Succeed(requirement);
            }
            // Note: We don't call context.Fail() to allow other handlers to run

            return Task.CompletedTask;
        }
    }
}
