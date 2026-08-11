using Microsoft.AspNetCore.Authorization;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Requirements
{
    /// <summary>
    /// Requirement: User can manage (create/modify/delete) users with the target role
    /// Resource: Target role (string) - the role we want to check authority over
    /// </summary>
    public class CanManageRoleRequirement : IAuthorizationRequirement
    {
    }
}
