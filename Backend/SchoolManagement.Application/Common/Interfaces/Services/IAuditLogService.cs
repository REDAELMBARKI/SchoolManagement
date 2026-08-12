namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IAuditLogService
{
    Task StoreAsync(
        string action,
        string entityName,
        Guid entityId,
        Guid branchId,
        object? oldValues = null,
        object? newValues = null,
        string? message = null,
        object? additionalInfo = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? severity = null,
        string? category = null,
        CancellationToken cancellationToken = default);
}
