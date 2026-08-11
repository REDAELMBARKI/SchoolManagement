

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
namespace SchoolManagement.CrossCutting.Identity.Authorizations.Extensions;



public static class AuthorizationExtensions
{

    public static IServiceCollection AddAppAuthorizations(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            AddGlobalRolesPolicies(options);
        });

        return services;
    }


 


    private static void AddGlobalRolesPolicies(AuthorizationOptions options)
    {
        options.AddPolicy("IsSuperAdmin", policy =>
        {
            policy.RequireRole("SuperAdmin");
        });

        options.AddPolicy("IsDirector", policy =>
        {
            policy.RequireRole("Director");
        });

        options.AddPolicy("IsAdministrator", policy =>
        {
            policy.RequireRole("Administrator");
        });

        options.AddPolicy("IsTeacher", policy =>
        {
            policy.RequireRole("Teacher");
        });


        options.AddPolicy("IsReceptionist", policy =>
        {
            policy.RequireRole("Receptionist");
        });
    }


   
}
