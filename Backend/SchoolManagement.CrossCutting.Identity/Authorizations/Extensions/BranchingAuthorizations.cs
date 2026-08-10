using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Extensions
{
    public static class BranchingAuthorizations
    {

        public static IServiceCollection AddBranchingAuthorizations(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                AddBranchAccessPolicies(options);
            });
            return services;
        }


        private static void AddBranchAccessPolicies(AuthorizationOptions options)
        {
            options.AddPolicy("IsSameBranch", policy =>
            { 
                policy.AddRequirements(new SameBranchRequirement());
            });
        }
    }
}
