using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Queries;

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
            .Include(e => e.Plan)
            .Include(e => e.Payments)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Subject)
            .Include(e => e.Group)
            .Include(e => e.Branch)
            .Include(e => e.Plan)
            .Include(e => e.Payments)
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
}
