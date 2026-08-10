using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Common.Interfaces;
using System.Security.Claims;

namespace SchoolManagement.Infrastructure.Data
{
    public class CurrentUserContext : ICurrentUserContext
    {
        public IHttpContextAccessor _httpContext;
        public Guid BranchId { get; }

        public string Role {  get; }
        public Guid NameIdentifier { get; }

        public CurrentUserContext(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            var userIdString = GetNameIdentifier();
            var branchIdString = GetBranchId();
            var userRole = GetUserRole();
            NameIdentifier = userIdString;
            BranchId = branchIdString;
        }

        private string  GetUserRole()
        {
            var userRole = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            if(string.IsNullOrEmpty(userRole))
            {
                throw new InvalidOperationException("User Role is Missing from the User context");
            }

            return userRole;
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

        private Guid GetBranchId()
        {

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
}
