using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Api.Controllers.Common;

/// <summary>
/// Base controller that applies branch-level access control to all actions.
/// SuperAdmin bypasses this check.
/// All controllers that need branch isolation should inherit from this.
/// </summary>
[ApiController]
[Authorize] // Requires authentication
public abstract class SecureControllerBase : ControllerBase
{
    // All child controllers automatically get:
    // - Authentication required
    // - Branch-level access control (via filter)
    
    // Controllers can override with [AllowAnonymous] for public endpoints
}
