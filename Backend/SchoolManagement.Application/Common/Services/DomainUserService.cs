using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Dtos.Commands;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Domain.Common.Utils;

namespace SchoolManagement.Application.Common.Services;

public class DomainUserService : IDomainUserService
{
    private readonly IUserRepository _repository;
    private readonly IUserQueryService _queryService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger<DomainUserService> _logger;

    public DomainUserService(
        IUserRepository repository,
        IUserQueryService queryService,
        IAuditLogService auditLogService,
        ICurrentUserContext currentUserContext,
        ILogger<DomainUserService> logger)
    {
        _repository = repository;
        _queryService = queryService;
        _auditLogService = auditLogService;
        _currentUserContext = currentUserContext;
        _logger = logger;
    }

    public async Task<DomainUserResponseDto> CreateAsync(DomainUserCommand command)
    {
        _logger.LogDebug("=== DomainUserService.CreateAsync START ===");
        _logger.LogDebug("Command.BranchId: {BranchId}", command.BranchId);
        _logger.LogDebug("Command.Email: {Email}", command.Email);
        _logger.LogDebug("Command.Role: {Role}", command.Role);
        _logger.LogDebug("CurrentUser.BranchId: {CurrentBranchId}", _currentUserContext.BranchId);
        _logger.LogDebug("CurrentUser.Role: {CurrentRole}", _currentUserContext.Role);

        // Validation: BranchId is required (no SuperAdmin creation via API)
        if (command.BranchId == Guid.Empty)
        {
            throw new DomainException("BranchId is required. SuperAdmin can only be created via database seeding.");
        }

        // Validation: SuperAdmin role is NOT allowed via API
        if (command.Role == "SuperAdmin")
        {
            throw new DomainException("SuperAdmin cannot be created via API. Only one SuperAdmin exists (seeded in database).");
        }

        // Validation: Check if email already exists
        if (await _repository.ExistsByEmailAsync(command.Email))
        {
            throw new DomainException($"User with email {command.Email} already exists.");
        }

        // Validation: Director can only create for their branch
        if (_currentUserContext.Role == "Director" && _currentUserContext.BranchId != Guid.Empty)
        {
            if (command.BranchId != _currentUserContext.BranchId )
            {
                throw new ForbiddenException("Director can only create users for their own branch.");
            }

            // Director cannot create Director
            if (command.Role == "Director")
            {
                throw new ForbiddenException("Director cannot create another Director role.");
            }
        }

        // Generate slug
        var baseSlug = $"{command.FirstName}-{command.LastName}-{command.Email}".ToLowerInvariant().Replace(" ", "-");
        command.Slug = await CustomSluger.Slug(
            async (slug) => await _repository.ExistsBySlugAsync(slug),
            baseSlug
        );

        // Create DomainUser (ApplicationUserId must already be set by caller)
        var domainUser = UserMapper.ToDomain(command);
        _logger.LogDebug("DomainUser created in memory - Id: {UserId}, ApplicationUserId: {AppUserId}, BranchId: {BranchId}", 
            domainUser.Id, domainUser.ApplicationUserId, domainUser.BranchId);
        
        await _repository.AddAsync(domainUser);
        _logger.LogDebug("DomainUser saved to database - Id: {UserId}, ApplicationUserId: {AppUserId}", 
            domainUser.Id, domainUser.ApplicationUserId);

        await _auditLogService.StoreAsync(
                action: AuditLog.CreateAction(),
                entityName: "DomainUser",
                entityId: domainUser.Id,
                branchId: domainUser.BranchId,
                newValues: CreateAuditSnapshot(domainUser));

        _logger.LogDebug("Querying user back from database...");
        var result = await _queryService.GetResponseByIdAsync(domainUser.Id);
        
        if (result == null)
        {
            _logger.LogError("USER NOT FOUND IN QUERY! UserId: {UserId}, SavedBranchId: {SavedBranchId}, CurrentUserBranchId: {CurrentBranchId}", 
                domainUser.Id, domainUser.BranchId, _currentUserContext.BranchId);
            throw new NotFoundException($"User created but not found in query.");
        }

        _logger.LogDebug("User found in query - Id: {UserId}, BranchId: {BranchId}", result.Id, result.BranchId);
        _logger.LogDebug("=== DomainUserService.CreateAsync END ===");
        
        return result;
    }

    public async Task<DomainUserResponseDto> UpdateAsync(Guid id, UpdateDomainUserCommand command)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }

        // Authorization: Users can update their own profile, SuperAdmin can update anyone, Director can update their branch users
        if (_currentUserContext.NameIdentifier != id && _currentUserContext.Role != "SuperAdmin")
        {
            if (_currentUserContext.Role == "Director" && 
                user.BranchId == _currentUserContext.BranchId)
            {
                // Director can update users in their branch
            }
            else
            {
                throw new ForbiddenException("You do not have permission to update this user.");
            }
        }

        var oldValues = CreateAuditSnapshot(user);

        // Check if name or phone changed - regenerate slug if needed
        bool nameOrPhoneChanged = user.FirstName != command.FirstName || 
                                   user.LastName != command.LastName || 
                                   user.Phone != command.Phone;

        if (nameOrPhoneChanged)
        {
            var baseSlug = $"{command.FirstName}-{command.LastName}-{command.Phone ?? user.Email?.Value}".ToLowerInvariant().Replace(" ", "-");
            command.Slug = await CustomSluger.Slug(
                async (slug) => await _repository.ExistsBySlugAsync(slug),
                baseSlug
            );
            user.UpdateSlug(command.Slug);
        }

        // Update person properties
        user.UpdateFirstName(command.FirstName);
        user.UpdateLastName(command.LastName);
        if (command.Phone != null) user.UpdatePhone(command.Phone);
        if (command.DateOfBirth.HasValue) user.UpdateDateOfBirth(command.DateOfBirth.Value);
        if (command.GenderId.HasValue) user.UpdateGenderId(command.GenderId.Value);

        await _repository.UpdateAsync(user);

        // Audit log: Track for all users (SuperAdmin uses SYSTEM_BRANCH_ID)
        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "DomainUser",
            entityId: id,
            branchId: user.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(user));

        return await _queryService.GetResponseByIdAsync(id) 
            ?? throw new NotFoundException("User not found after update.");
    }

    public async Task DeleteAsync(Guid id)
    {
        // Only SuperAdmin can delete users
        if (_currentUserContext.Role != "SuperAdmin")
        {
            throw new ForbiddenException("Only SuperAdmin can delete users.");
        }

        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(user);

        // Soft delete DomainUser
        await _repository.DeleteAsync(id);

        // Note: ApplicationUser deletion should be handled by AccountController/AuthService
        // This service only handles DomainUser (business layer)

        // Audit log: Track for all users (SuperAdmin uses SYSTEM_BRANCH_ID)
        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "DomainUser",
            entityId: id,
            branchId: user.BranchId,
            oldValues: oldValues);
    }

    public async Task<DomainUserResponseDto> GetByIdAsync(Guid id)
    {
        var user = await _queryService.GetResponseByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {id} not found.");
        }

        return user;
    }

    public async Task<List<DomainUserResponseDto>> GetAllAsync()
    {
        return await _queryService.GetAllResponsesAsync();
    }

    public async Task<DomainUserResponseDto> AssignBranchAsync(Guid userId, AssignBranchCommand command)
    {
        // Only SuperAdmin can assign branches
        if (_currentUserContext.Role != "SuperAdmin")
        {
            throw new ForbiddenException("Only SuperAdmin can assign branches.");
        }

        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        var oldValues = CreateAuditSnapshot(user);

        // Update branch
        user.UpdateBranch(command.BranchId);
        await _repository.UpdateAsync(user);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "DomainUser",
            entityId: userId,
            branchId: command.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(user));

        return await _queryService.GetResponseByIdAsync(userId) 
            ?? throw new NotFoundException("User not found after branch assignment.");
    }

    public async Task<DomainUserResponseDto> RemoveBranchAsync(Guid userId)
    {
        // Only SuperAdmin can remove branches (effectively making user SuperAdmin by setting SYSTEM_BRANCH_ID)
        if (_currentUserContext.Role != "SuperAdmin")
        {
            throw new ForbiddenException("Only SuperAdmin can remove branch assignments.");
        }

        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        var oldValues = CreateAuditSnapshot(user);
        var oldBranchId = user.BranchId; // Capture before removal

        // Set branch to SYSTEM_BRANCH_ID - user becomes SuperAdmin-like
        user.UpdateBranch(Branch.SYSTEM_BRANCH_ID);
        await _repository.UpdateAsync(user);

        // Audit log: Track the branch change
        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "DomainUser",
            entityId: userId,
            branchId: oldBranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(user));

        return await _queryService.GetResponseByIdAsync(userId) 
            ?? throw new NotFoundException("User not found after branch removal.");
    }

    public async Task<DomainUserResponseDto> ActivateAsync(Guid userId)
    {
        // SuperAdmin or Director (for their branch) can activate users
        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        if (_currentUserContext.Role != "SuperAdmin")
        {
            if (_currentUserContext.Role == "Director" && user.BranchId == _currentUserContext.BranchId)
            {
                // Director can activate users in their branch
            }
            else
            {
                throw new ForbiddenException("You do not have permission to activate this user.");
            }
        }

        var oldValues = CreateAuditSnapshot(user);

        user.Activate();
        await _repository.UpdateAsync(user);

        // Audit log: Track for all users (SuperAdmin uses SYSTEM_BRANCH_ID)
        await _auditLogService.StoreAsync(
            action: "Activate",
            entityName: "DomainUser",
            entityId: userId,
            branchId: user.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(user));

        return await _queryService.GetResponseByIdAsync(userId) 
            ?? throw new NotFoundException("User not found after activation.");
    }

    public async Task<DomainUserResponseDto> DeactivateAsync(Guid userId)
    {
        // SuperAdmin or Director (for their branch) can deactivate users
        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        if (_currentUserContext.Role != "SuperAdmin")
        {
            if (_currentUserContext.Role == "Director" && user.BranchId == _currentUserContext.BranchId)
            {
                // Director can deactivate users in their branch
            }
            else
            {
                throw new ForbiddenException("You do not have permission to deactivate this user.");
            }
        }

        var oldValues = CreateAuditSnapshot(user);

        user.Deactivate();
        await _repository.UpdateAsync(user);

        // Audit log: Track for all users (SuperAdmin uses SYSTEM_BRANCH_ID)
        await _auditLogService.StoreAsync(
            action: "Deactivate",
            entityName: "DomainUser",
            entityId: userId,
            branchId: user.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(user));

        return await _queryService.GetResponseByIdAsync(userId) 
            ?? throw new NotFoundException("User not found after deactivation.");
    }

    public async Task<List<DomainUserResponseDto>> GetByBranchIdAsync(Guid branchId)
    {
        return await _queryService.GetByBranchIdAsync(branchId);
    }

    public async Task<List<DomainUserResponseDto>> GetByRoleAsync(string role)
    {
        return await _queryService.GetByRoleAsync(role);
    }

    private static object CreateAuditSnapshot(DomainUser user)
    {
        return new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Slug,
            Email = user.Email?.Value,
            user.Phone,
            user.DateOfBirth,
            user.GenderId,
            user.Role,
            user.BranchId,
            user.IsActive,
            user.LastActiveAt
        };
    }


    public async Task<DomainUserResponseDto> GetByApplicationUserIdAsync(string applicationUserId)
    {
        var user = await _repository.GetByApplicationUserIdAsync(applicationUserId);
        
        if (user == null)
        {
            throw new NotFoundException($"DomainUser with ApplicationUserId '{applicationUserId}' not found.");
        }

        return UserMapper.ToResponse(user);
    }

    public async Task<DomainUserResponseDto> ConvertToStaffAsync(ConvertToStaffCommand command)
    {
        _logger.LogDebug("Converting user {UserId} to staff role {Role}", command.UserId, command.Role);

        // Get the user
        var user = await _repository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {command.UserId} not found.");
        }

        // Validation: Cannot convert SuperAdmin
        if (user.Role == RoleHelper.SuperAdmin)
        {
            throw new DomainException("SuperAdmin cannot be converted to another role.");
        }

        // Validation: New role cannot be SuperAdmin
        if (command.Role == RoleHelper.SuperAdmin)
        {
            throw new DomainException("Cannot convert user to SuperAdmin role.");
        }

        // Authorization: Only SuperAdmin and Director can convert users
        if (_currentUserContext.Role != RoleHelper.SuperAdmin && _currentUserContext.Role != RoleHelper.Director)
        {
            throw new ForbiddenException("Only SuperAdmin and Director can convert users to staff.");
        }

        // Authorization: Director can only convert users in their branch
        if (_currentUserContext.Role == RoleHelper.Director)
        {
            if (user.BranchId != _currentUserContext.BranchId)
            {
                throw new ForbiddenException("Director can only convert users from their own branch.");
            }

            // Director cannot create Director role
            if (command.Role == RoleHelper.Director)
            {
                throw new ForbiddenException("Director cannot convert users to Director role.");
            }
        }

        var oldValues = CreateAuditSnapshot(user);

        // Update role and branch
        user.UpdateRole(command.Role);
        user.UpdateBranch(command.BranchId);

        await _repository.UpdateAsync(user);

        await _auditLogService.StoreAsync(
            action: "ConvertToStaff",
            entityName: "DomainUser",
            entityId: user.Id,
            branchId: command.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(user));

        _logger.LogInformation("User {UserId} converted to staff role {Role} in branch {BranchId}", 
            command.UserId, command.Role, command.BranchId);

        return await _queryService.GetResponseByIdAsync(user.Id) 
            ?? throw new NotFoundException("User not found after conversion.");
    }

}