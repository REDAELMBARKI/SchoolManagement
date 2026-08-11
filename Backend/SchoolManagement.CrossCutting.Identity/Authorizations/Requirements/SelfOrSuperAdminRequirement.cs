using Microsoft.AspNetCore.Authorization;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Requirements
{
    /// <summary>
    /// Requirement: User can access their own data OR is SuperAdmin
    /// Resource: The target ApplicationUserId (string)
    /// </summary>
    public class SelfOrSuperAdminRequirement : IAuthorizationRequirement
    {
    }
}
