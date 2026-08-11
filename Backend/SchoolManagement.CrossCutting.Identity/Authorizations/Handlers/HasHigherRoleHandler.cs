using Microsoft.AspNetCore.Authorization;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
using SchoolManagement.Domain.Common.Utils;
using System.Security.Claims;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Handlers
{
    public class HasHigherRoleHandler : AuthorizationHandler<HasHigherRoleRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasHigherRoleRequirement requirement, string resource)
        {
            var contextRole = context.User.FindFirstValue(ClaimTypes.Role);
            var superAdminManagedRole = new [] { RoleHelper.Administrator, RoleHelper.Director, RoleHelper.Reciptionest, RoleHelper.Teacher , RoleHelper.User };
            var directorManagedRole = new [] { RoleHelper.Administrator , RoleHelper.Reciptionest, RoleHelper.Teacher, RoleHelper.User };
            var administratorManagedRole = new [] {  RoleHelper.Teacher, RoleHelper.User };
            var hasSucceded = false;

            if (!hasSucceded && contextRole == RoleHelper.SuperAdmin  && superAdminManagedRole.Contains(resource))
            {
                   hasSucceded = true;
                   context.Succeed(requirement);
            }


            if (!hasSucceded && contextRole == RoleHelper.Director && directorManagedRole.Contains(resource))
            {

                hasSucceded = true;
                context.Succeed(requirement);
            }


            if (!hasSucceded &&  contextRole == RoleHelper.Administrator && administratorManagedRole.Contains(resource))
            {
                hasSucceded = true;
                context.Succeed(requirement);
            }


            if (!hasSucceded) context.Fail();

            return Task.CompletedTask;


        }
    }
}
