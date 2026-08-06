using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Common.Queries;

public class WhatsAppMessageQueryService : IWhatsAppMessageQueryService
{
    private readonly AppDbContext _context;

    public WhatsAppMessageQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WhatsAppMessage>> GetAllAsync()
    {
        return await _context.WhatsAppMessages
            .Include(m => m.Branch)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<WhatsAppMessage?> GetByIdAsync(Guid id)
    {
        return await _context.WhatsAppMessages
            .Include(m => m.Branch)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.WhatsAppMessages
            .AnyAsync(m => m.Id == id);
    }

    public async Task<List<WhatsAppMessage>> GetMessagesByEntityAsync(string entityType, Guid entityId)
    {
        return await _context.WhatsAppMessages
            .Include(m => m.Branch)
            .Where(m => m.EntityType == entityType && m.EntityId == entityId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}
