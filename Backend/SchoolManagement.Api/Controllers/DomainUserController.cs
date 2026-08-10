using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/domain-users")]
public class DomainUserController : ControllerBase
{
    private readonly IDomainUserService _service;

    public DomainUserController(IDomainUserService service)
    {
        _service = service;
    }

    // 1. GET /api/domain-users - Get all users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // 2. GET /api/domain-users/{id} - Get user by ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
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

    // 3. PUT /api/domain-users/{id} - Update user profile
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
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

    // 4. DELETE /api/domain-users/{id} - Delete user (soft delete)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
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

    // 5. POST /api/domain-users/{id}/assign-branch - Assign user to branch
    [HttpPost("{id:guid}/assign-branch")]
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

    // 6. POST /api/domain-users/{id}/remove-branch - Remove user from branch
    [HttpPost("{id:guid}/remove-branch")]
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

    // 7. POST /api/domain-users/{id}/activate - Activate user
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        try
        {
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

    // 8. POST /api/domain-users/{id}/deactivate - Deactivate user
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
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

    // 9. GET /api/domain-users/branch/{branchId} - Get all users in specific branch
    [HttpGet("branch/{branchId:guid}")]
    public async Task<IActionResult> GetByBranch(Guid branchId)
    {
        try
        {
            var result = await _service.GetByBranchIdAsync(branchId);
            return Ok(result);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    // 10. GET /api/domain-users/role/{role} - Get all users with specific role
    [HttpGet("role/{role}")]
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
