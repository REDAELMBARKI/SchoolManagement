using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.CrossCutting.Identity.Authorizations.Handlers;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Extensions
{
    public static class AccountsAuthorizations
    {

        public static IServiceCollection AddAccountsAuthorizations(this IServiceCollection services)
        {

            services.AddAuthorization(options =>
            {
                // Claim-based policies
                options.AddPolicy("users:view", policy => policy.RequireClaim("users:view"));
                options.AddPolicy("users:edit", policy => policy.RequireClaim("users:edit"));
                options.AddPolicy("users:delete", policy => policy.RequireClaim("users:delete"));
                options.AddPolicy("users:manageRoles", policy => policy.RequireClaim("users:manageRoles"));

                // Role-based policies
                options.AddPolicy("IsSuperAdmin", policy => policy.RequireRole("SuperAdmin"));
                options.AddPolicy("IsDirectorOrAbove", policy => policy.RequireRole("SuperAdmin", "Director"));
                options.AddPolicy("IsAdministratorOrAbove", policy => policy.RequireRole("SuperAdmin", "Director", "Administrator"));
                options.AddPolicy("IsReceptionistOrAbove", policy => policy.RequireRole("SuperAdmin", "Director", "Administrator", "Receptionist"));

                // Resource-based policies (custom requirements)
                options.AddPolicy("HasHigherRole", policy => policy.Requirements.Add(new HasHigherRoleRequirement()));
                options.AddPolicy("CanManageRole", policy => policy.Requirements.Add(new CanManageRoleRequirement()));
                options.AddPolicy("SelfOrSuperAdmin", policy => policy.Requirements.Add(new SelfOrSuperAdminRequirement()));

            });

            // Register authorization handlers
            services.AddScoped<IAuthorizationHandler, HasHigherRoleHandler>();
            services.AddScoped<IAuthorizationHandler, CanManageRoleHandler>();
            services.AddScoped<IAuthorizationHandler, SelfOrSuperAdminHandler>();

            return services;
        
        }
    }
}
