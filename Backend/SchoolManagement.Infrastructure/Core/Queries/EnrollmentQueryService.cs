using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Core.Interfaces.Queries;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Core.Enums;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Core.Queries;

public class EnrollmentQueryService : IEnrollmentQueryService
{
    private readonly AppDbContext _context;

    public EnrollmentQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Subject)
            .Include(e => e.Group)
            .Include(e => e.Branch)
            .Include(e => e.Payments)
            .Include(e => e.EnrollmentPlans)
                .ThenInclude(ep => ep.Plan)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Subject)
            .Include(e => e.Group)
            .Include(e => e.Branch)
            .Include(e => e.Payments)
            .Include(e => e.EnrollmentPlans)
                .ThenInclude(ep => ep.Plan)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.Id == id);
    }

    public async Task<List<EnrollmentResponseDto>> GetAllResponsesAsync()
    {
        var enrollments = await GetAllAsync();
        return enrollments.Select(EnrollmentMapper.ToResponse).ToList();
    }

    public async Task<EnrollmentResponseDto?> GetResponseByIdAsync(Guid id)
    {
        var enrollment = await GetByIdAsync(id);
        return enrollment == null ? null : EnrollmentMapper.ToResponse(enrollment);
    }

    public async Task<bool> HasActiveEnrollmentForStudentSubjectAsync(Guid studentId, Guid subjectId)
    {
        if (studentId == Guid.Empty) return false;
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId
                        && e.SubjectId == subjectId
                        && e.Status == EnrollmentStatus.Active);
    }

    public async Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Enrollments.Where(e => e.StudentId == studentId)
            .Include(e => e.Subject)
            .Include(e => e.Group)
            .Include(e => e.Branch)
            .Include(e => e.Payments)
            .Include(e => e.EnrollmentPlans)
                .ThenInclude(ep => ep.Plan)
            .ToListAsync();
    }
}
