using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Services.AuditLogs;

public class AuditLogService : IAuditLogService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task StoreAsync(
        string action,
        string entityName,
        Guid entityId,
        Guid branchId,
        object? oldValues = null,
        object? newValues = null,
        CancellationToken cancellationToken = default)
    {
        if (branchId == Guid.Empty)
        {
            return;
        }

        var changedBy = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var auditLog = AuditLog.Create(
            entityName: entityName,
            entityId: entityId,
            action: action,
            oldValues: Serialize(oldValues),
            newValues: Serialize(newValues),
            changedBy: changedBy,
            branchId: branchId);

        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string? Serialize(object? values)
    {
        if (values is null)
        {
            return null;
        }

        return JsonSerializer.Serialize(values, JsonOptions);
    }
}
