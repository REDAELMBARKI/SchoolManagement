

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
        options.AddPolicy("SuperAdminOnly", policy =>
        {
            policy.RequireRole("SuperAdmin");
        });


        options.AddPolicy("Admin", policy =>
        {
            policy.RequireRole("Admin");
        });

        options.AddPolicy("Teacher", policy =>
        {
            policy.RequireRole("Teacher");
        });


        options.AddPolicy("FrontDesk", policy =>
        {
            policy.RequireRole("FrontDesk");
        });
    }


   
}
