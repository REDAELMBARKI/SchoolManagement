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
using SchoolManagement.Application.Academic.Interfaces.Queries;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Academic.Queries;

public class SubjectQueryService : ISubjectQueryService
{
    private readonly AppDbContext _context;

    public SubjectQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Subject>> GetAllAsync()
    {
        return await _context.Subjects
            .Include(s => s.Branch)
            .ToListAsync();
    }

    public async Task<Subject?> GetByIdAsync(Guid id)
    {
        return await _context.Subjects
            .Include(s => s.Branch)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Subjects
            .AnyAsync(s => s.Id == id);
    }

    public async Task<List<SubjectResponseDto>> GetAllResponsesAsync()
    {
        var subjects = await GetAllAsync();
        return subjects.Select(SubjectMapper.ToResponse).ToList();
    }

    public async Task<SubjectResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var subject = await GetByIdAsync(id);
        return subject == null ? null : SubjectMapper.ToResponse(subject);
    }
}
