namespace SchoolManagement.Application.Interfaces.Services;

public interface IAuditLogService
{
    Task StoreAsync(
        string action,
        string entityName,
        Guid entityId,
        Guid branchId,
        object? oldValues = null,
        object? newValues = null,
        CancellationToken cancellationToken = default);
}
