using Microsoft.AspNetCore.Authorization;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Requirements
{
    public class HasHigherRoleRequirement : IAuthorizationRequirement
    {
    }
}
