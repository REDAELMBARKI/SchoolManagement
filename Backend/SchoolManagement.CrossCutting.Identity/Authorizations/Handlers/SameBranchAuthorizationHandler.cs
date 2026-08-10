using Microsoft.AspNetCore.Authorization;
using SchoolManagement.CrossCutting.Identity.Authorizations.Requirements;
using SchoolManagement.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Handlers
{
    public class SameBranchAuthorizationHandler : AuthorizationHandler<SameBranchRequirement, Branch>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context , SameBranchRequirement requirement, Branch resource) 
        {
            if(resource is null)
            {
                AuthorizationResult.Success();
            }
           
            if(context.User.FindFirstValue("BranchId") == resource!.Id.ToString())
             {
                AuthorizationResult.Success();
             }

            return Task.CompletedTask;
        }

      
    }
}
