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
        Guid branchId,
        object? oldValues = null,
        object? newValues = null,
        string? message = null,
        object? additionalData = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? severity = null,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var changedBy = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasRole = httpContext?.User.FindFirstValue(ClaimTypes.Role);

        // AuditLog.Create handles the Guid.Empty → SYSTEM_BRANCH_ID fallback
        var auditLog = AuditLog.Create(
            entityName: entityName,
            entityId: entityId,
            action: action,
            oldValues: Serialize(oldValues),
            newValues: Serialize(newValues),
            changedBy: changedBy,
            hasRole: hasRole,
            branchId: branchId, // Fallback handled in entity
            additionalData: Serialize(additionalData),
            message: message,
            ipAddress: ipAddress ?? httpContext?.Connection.RemoteIpAddress?.ToString(),
            userAgent: userAgent ?? httpContext?.Request.Headers["User-Agent"].ToString(),
            severity: severity,
            category: category
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
