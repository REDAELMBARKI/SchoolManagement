using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/domain-users")]
[Authorize] 
public class DomainUserController : ControllerBase
{
    private readonly IDomainUserService _service;
    private readonly IAuthorizationService _authorizationService;

    public DomainUserController(IDomainUserService service, IAuthorizationService authorizationService)
    {
        _service = service;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    [Authorize(Policy = "IsAdministratorOrAbove")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "IsAdministratorOrAbove")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);

            var branchCheck = await _authorizationService.AuthorizeAsync(
                User, 
                result.BranchId, 
                "IsSameBranch"
            );

            if (!branchCheck.Succeeded)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "IsAdministratorOrAbove")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            var user = await _service.GetByIdAsync(id);

            var branchCheck = await _authorizationService.AuthorizeAsync(
                User, 
                user.BranchId, 
                "IsSameBranch"
            );

            if (!branchCheck.Succeeded)
            {
                return NotFound();
            }

            var roleCheck = await _authorizationService.AuthorizeAsync(
                User, 
                user.Role, 
                "CanManageRole"
            );

            if (!roleCheck.Succeeded)
            {
                return NotFound();
            }

            var command = new UpdateDomainUserCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                GenderId = request.GenderId
            };

            var result = await _service.UpdateAsync(id, command);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "IsDirectorOrAbove")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var user = await _service.GetByIdAsync(id);

            var branchCheck = await _authorizationService.AuthorizeAsync(
                User, 
                user.BranchId, 
                "IsSameBranch"
            );

            if (!branchCheck.Succeeded)
            {
                return NotFound();
            }

            var roleCheck = await _authorizationService.AuthorizeAsync(
                User, 
                user.Role, 
                "CanManageRole"
            );

            if (!roleCheck.Succeeded)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/assign-branch")]
    [Authorize(Policy = "IsSuperAdmin")]
    public async Task<IActionResult> AssignBranch(Guid id, [FromBody] AssignBranchRequestDto request)
    {
        try
        {
            var command = new AssignBranchCommand
            {
                BranchId = request.BranchId
            };

            var result = await _service.AssignBranchAsync(id, command);
            return Ok(new 
            { 
                message = "User assigned to branch successfully. User must re-login to get new JWT token.",
                user = result 
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/remove-branch")]
    [Authorize(Policy = "IsSuperAdmin")]
    public async Task<IActionResult> RemoveBranch(Guid id)
    {
        try
        {
            var result = await _service.RemoveBranchAsync(id);
            return Ok(new 
            { 
                message = "Branch removed from user successfully.",
                user = result 
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/activate")]
    [Authorize(Policy = "IsAdministratorOrAbove")]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
            var user = await _service.GetByIdAsync(id);

            var branchCheck = await _authorizationService.AuthorizeAsync(
                User, 
                user.BranchId, 
                "IsSameBranch"
            );

            if (!branchCheck.Succeeded)
            {
                return NotFound();
            }

            var roleCheck = await _authorizationService.AuthorizeAsync(
                User, 
                user.Role, 
                "CanManageRole"
            );

            if (!roleCheck.Succeeded)
            {
                return NotFound();
            }

            var result = await _service.ActivateAsync(id);
            return Ok(new 
            { 
                message = "User activated successfully.",
                user = result 
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Policy = "IsAdministratorOrAbove")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var user = await _service.GetByIdAsync(id);

            var branchCheck = await _authorizationService.AuthorizeAsync(
                User, 
                user.BranchId, 
                "IsSameBranch"
            );

            if (!branchCheck.Succeeded)
            {
                return NotFound();
            }

            var roleCheck = await _authorizationService.AuthorizeAsync(
                User, 
                user.Role, 
                "CanManageRole"
            );

            if (!roleCheck.Succeeded)
            {
                return NotFound();
            }

            var result = await _service.DeactivateAsync(id);
            return Ok(new 
            { 
                message = "User deactivated successfully.",
                user = result 
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpGet("branch/{branchId:guid}")]
    [Authorize(Policy = "IsAdministratorOrAbove")]
    public async Task<IActionResult> GetByBranch(Guid branchId)
    {
        try
        {
            var branchCheck = await _authorizationService.AuthorizeAsync(
                User, 
                branchId, 
                "IsSameBranch"
            );

            if (!branchCheck.Succeeded)
            {
                return Forbid();
            }

            var result = await _service.GetByBranchIdAsync(branchId);
            return Ok(result);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpGet("role/{role}")]
    [Authorize(Policy = "IsAdministratorOrAbove")]
    public async Task<IActionResult> GetByRole(string role)
    {
        try
        {
            var result = await _service.GetByRoleAsync(role);
            return Ok(result);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }
}
