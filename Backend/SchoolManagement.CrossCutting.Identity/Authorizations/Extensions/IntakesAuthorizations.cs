using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Extensions
{
    public static class IntakesAuthorizations
    {


        public static IServiceCollection AddIntakesAuthorizations(this IServiceCollection services)
        {

            services.AddAuthorization(options =>
            {
                AddIntakesPolicies(options);
            });
            return services;
        }


        private static void AddIntakesPolicies(AuthorizationOptions options)
        {
            options.AddPolicy("IntakeCreate", policy =>
            {
                policy.RequireClaim("Permission", "Intake.Create");
            });
            options.AddPolicy("IntakeRead", policy =>
            {
                policy.RequireClaim("Permission", "Intake.Read");
            });
            options.AddPolicy("IntakeUpdate", policy =>
            {
                policy.RequireClaim("Permission", "Intake.Update");
            });
            options.AddPolicy("IntakeDelete", policy =>
            {
                policy.RequireClaim("Permission", "Intake.Delete");
            });
        }
    }
}
