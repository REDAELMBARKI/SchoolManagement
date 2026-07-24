using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SchoolManagement.Infrastructure.Data
{
    public class CurrentUserContext : ICurrentUserContext
    {
        public IHttpContextAccessor _httpContext;
        public Guid BranchId { get; }
        public CurrentUserContext(IHttpContextAccessor httpContext) {
            _httpContext = httpContext;
            var branchIdString = _httpContext.HttpContext?.User.FindFirstValue("BranchId");

            if (string.IsNullOrEmpty(branchIdString))
            {
                throw new InvalidOperationException(
                    "BranchId claim is missing from the current user's identity . check the login/claims setup ");
            }

            if (!Guid.TryParse(branchIdString, out var branchId))
            {
                throw new InvalidOperationException(
                    $"BranchId claim value '{branchIdString}' is not a valid GUID.");
            }

            BranchId = branchId;
        }

    }
}
