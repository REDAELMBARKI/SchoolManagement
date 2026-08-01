
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Queries;

public class GroupQueryService : IGroupQueryService
{
    private readonly AppDbContext _context;

    public GroupQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Group>> GetAllAsync()
    {
        return await _context.Groups
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Teachers)
            .ToListAsync();
    }

    public async Task<Group?> GetByIdAsync(Guid id)
    {
        return await _context.Groups
            .Include(g => g.Level)
            .Include(g => g.Subject)
            .Include(g => g.Schedule)
            .Include(g => g.Teachers)
            .Include(g => g.Enrollments)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Groups
                      .AnyAsync(g => g.Id == id);
    }

    public async Task<List<GroupResponseDto>> GetAllResponsesAsync()
    {
        var groups = await GetAllAsync();
        return groups.Select(GroupMapper.ToResponse).ToList();
    }

    public async Task<GroupResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var group = await GetByIdAsync(id);
        return group == null ? null : GroupMapper.ToResponse(group);
    }

    public async Task<List<Group>> GetAvailableGroupsByLevelSubjectBranch(Guid levelId, Guid subjectId, Guid branchId)
    {
        return await _context.Groups
                    .Include(g => g.Enrollments)
                    .Include(g => g.Schedule)
                    .Where(g => g.LevelId == levelId)
                    .Where(g => g.SubjectId == subjectId)
                    .Where(g => g.BranchId == branchId)
                    .Where(g => g.Capacity > g.Enrollments.Count(e => e.Status == EnrollmentStatus.Active))
                    .ToListAsync();
    }
}

