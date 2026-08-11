using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.CrossCutting.Identity.Authorizations.Handlers;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Extensions;

public static class BranchingAuthorizations
{
    public static IServiceCollection AddBranchingAuthorizations(this IServiceCollection services)
    {

        services.AddAuthorization(options =>
        {  
            AddBranchAccessPolicies(options);
        });
        
        services.AddScoped<IAuthorizationHandler, SameBranchAuthorizationHandler>();
        return services;
    }

    private static void AddBranchAccessPolicies(AuthorizationOptions options)
    {
        // Resource-based: Check if target BranchId matches user's BranchId (SuperAdmin bypasses)
        options.AddPolicy("IsSameBranch", policy =>
        {
            policy.AddRequirements(new SameBranchRequirement());
        });

        // Claim-based: Requires BranchId claim and specific roles
        options.AddPolicy("HasBranchAccess", policy =>
        {
            policy.RequireClaim("BranchId");
            policy.RequireRole("Director" , "Administrator" , "Receptionist" , "Teacher");
        });
    }
}
