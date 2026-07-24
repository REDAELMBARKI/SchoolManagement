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
        public Guid NameIdentifier {  get; }

        public CurrentUserContext(IHttpContextAccessor httpContext) {
            _httpContext = httpContext;
            var userIdString = GetNameIdentifier();
            var branchIdString = GetBranchId();
            NameIdentifier = userIdString;
            BranchId = branchIdString;
        }

        private Guid GetNameIdentifier()
        {
            var userIdString = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                throw new InvalidOperationException(
                    "NameIdentifier claim is missing from the current user's identity . check the login/claims setup ");
            }

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new InvalidOperationException(
                    $"NameIdentifier claim value '{userIdString}' is not a valid GUID.");
            }

            return userId;
        }

        private Guid GetBranchId() {

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

            return branchId;
        }
}
