using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Queries;

public class UserQueryService : IUserQueryService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger<UserQueryService> _logger;

    public UserQueryService(AppDbContext context, ICurrentUserContext currentUserContext, ILogger<UserQueryService> logger)
    {
        _context = context;
        _currentUserContext = currentUserContext;
        _logger = logger;
    }

    public async Task<List<DomainUser>> GetAllAsync()
    {
        var query = _context.Set<DomainUser>()
            .Include(u => u.Branch)
            .Include(u => u.Gender)
            .AsNoTracking()
            .Where(u => u.DeletedAt == null);

        // Branch filtering: SuperAdmin sees all, others see only their branch
        if (_currentUserContext.BranchId != Guid.Empty)
        {
            query = query.Where(u => u.BranchId == _currentUserContext.BranchId);
        }

        return await query.ToListAsync();
    }

    public async Task<DomainUser?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("=== UserQueryService.GetByIdAsync START ===");
        _logger.LogDebug("Querying UserId: {UserId}", id);
        _logger.LogDebug("CurrentUser.BranchId: {CurrentBranchId}", _currentUserContext.BranchId);
        
        var user = await _context.Set<DomainUser>()
            .Include(u => u.Branch)
            .Include(u => u.Gender)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

        if (user == null)
        {
            _logger.LogWarning("User NOT found in database! UserId: {UserId}", id);
            return null;
        }

        _logger.LogDebug("User found in database - UserId: {UserId}, User.BranchId: {UserBranchId}", user.Id, user.BranchId);

        // Branch ownership validation
        if (_currentUserContext.BranchId != Guid.Empty &&
            user.BranchId != _currentUserContext.BranchId)
        {
            _logger.LogWarning("BRANCH MISMATCH! User.BranchId: {UserBranchId}, CurrentUser.BranchId: {CurrentBranchId} - Returning NULL", 
                user.BranchId, _currentUserContext.BranchId);
            return null;  // Forbidden - return null (will be handled by service)
        }

        _logger.LogDebug("Branch check passed - returning user");
        _logger.LogDebug("=== UserQueryService.GetByIdAsync END ===");
        return user;
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        var query = _context.Set<DomainUser>()
            .AsNoTracking()
            .Where(u => u.Id == id && u.DeletedAt == null);

        // Branch filtering
        if (_currentUserContext.BranchId != Guid.Empty)
        {
            query = query.Where(u => u.BranchId == _currentUserContext.BranchId);
        }

        return await query.AnyAsync();
    }

    public async Task<List<DomainUserResponseDto>> GetAllResponsesAsync()
    {
        var users = await GetAllAsync();
        return users.Select(u => UserMapper.ToResponse(
            u,
            branchName: u.Branch?.Name,
            genderName: u.Gender?.Name
        )).ToList();
    }
    public async Task<DomainUserResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user == null) return null;

        return UserMapper.ToResponse(
            user,
            branchName: user.Branch?.Name,
            genderName: user.Gender?.Name
        );
    }

    public async Task<DomainUser?> GetByEmailAsync(string email)
    {
        var query = _context.Set<DomainUser>()
            .AsNoTracking()
            .Where(u => u.Email != null && u.Email.Value == email && u.DeletedAt == null);

        // Branch filtering
        if (_currentUserContext.BranchId != Guid.Empty)
        {
            query = query.Where(u => u.BranchId == _currentUserContext.BranchId);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<DomainUser?> GetBySlugAsync(string slug)
    {
        var query = _context.Set<DomainUser>()
            .Include(u => u.Branch)
            .Include(u => u.Gender)
            .AsNoTracking()
            .Where(u => u.Slug == slug && u.DeletedAt == null);

        // Branch filtering
        if (_currentUserContext.BranchId != Guid.Empty)
        {
            query = query.Where(u => u.BranchId == _currentUserContext.BranchId);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<DomainUserResponseDto>> GetByBranchIdAsync(Guid branchId)
    {
        // Only SuperAdmin can query other branches
        if (_currentUserContext.BranchId != Guid.Empty && _currentUserContext.BranchId != branchId)
        {
            return new List<DomainUserResponseDto>();  // Forbidden
        }

        var users = await _context.Set<DomainUser>()
            .Include(u => u.Branch)
            .Include(u => u.Gender)
            .AsNoTracking()
            .Where(u => u.BranchId == branchId && u.DeletedAt == null)
            .ToListAsync();

        return users.Select(u => UserMapper.ToResponse(
            u,
            branchName: u.Branch?.Name,
            genderName: u.Gender?.Name
        )).ToList();
    }

    public async Task<List<DomainUserResponseDto>> GetByRoleAsync(string role)
    {
        var query = _context.Set<DomainUser>()
            .Include(u => u.Branch)
            .Include(u => u.Gender)
            .AsNoTracking()
            .Where(u => u.Role == role && u.DeletedAt == null);

        // Branch filtering
        if (_currentUserContext.BranchId != Guid.Empty)
        {
            query = query.Where(u => u.BranchId == _currentUserContext.BranchId);
        }

        var users = await query.ToListAsync();

        return users.Select(u => UserMapper.ToResponse(
            u,
            branchName: u.Branch?.Name,
            genderName: u.Gender?.Name
        )).ToList();
    }
}
