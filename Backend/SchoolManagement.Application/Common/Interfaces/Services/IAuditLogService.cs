namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IAuditLogService
{
    Task StoreAsync(
        string action,
        string entityName,
        Guid entityId,
        Guid branchId,  // REQUIRED - tracks the affected entity's branch (where changes happen)
        object? oldValues = null,
        object? newValues = null,
        string? message = null,
        object? additionalInfo = null,
        CancellationToken cancellationToken = default);
}
