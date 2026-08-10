using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolManagement.Infrastructure.Common.Services;

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
        Guid branchId,  // REQUIRED - tracks where the change happened (affected entity's branch)
        object? oldValues = null,
        object? newValues = null,
        string? message = null,
        object? additionalData = null,
        CancellationToken cancellationToken = default)
    {
        // Skip audit log if branchId is empty
        if (branchId == Guid.Empty)
        {
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var changedBy = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasRole = httpContext?.User.FindFirstValue(ClaimTypes.Role);

        var auditLog = AuditLog.Create(
            entityName: entityName,
            entityId: entityId,
            action: action,
            oldValues: Serialize(oldValues),
            newValues: Serialize(newValues),
            changedBy: changedBy,
            hasRole: hasRole,
            branchId: branchId,
            additionalData: Serialize(additionalData),
            message: message
            );

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
