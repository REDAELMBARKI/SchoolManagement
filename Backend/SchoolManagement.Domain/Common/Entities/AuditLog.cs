namespace SchoolManagement.Domain.Common.Entities;

public class AuditLog : BaseEntity
{
    public string EntityName { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty; // Create, Update, Delete, Waive, etc.
    public string? Message { get; private set; }
    public string? OldValues { get; private set; } // JSON
    public string? NewValues { get; private set; } // JSON
    public string? ChangedBy { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid BranchId { get; private set; }
    public string? AdditionalData { get; private set; } // JSON for any additional data
    public virtual Branch Branch { get; private set; } = null!;

    private AuditLog() { }

    public static AuditLog Create(
        string entityName,
        Guid entityId,
        string action,
        string? oldValues,
        string? newValues,
        string? changedBy,
        Guid branchId,
        string? additionalData,
        string? message = null)
    {
        return new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            Message = message,
            AdditionalData = additionalData,    
            OldValues = oldValues,
            NewValues = newValues,
            ChangedBy = changedBy,
            ChangedAt = DateTime.UtcNow,
            BranchId = branchId
        };
    }

    public void UpdateBranchId(Guid branchId)
    {
        BranchId = branchId;
    }

    public static string CreateAction()
    {
        return "Create";
    }

    public static string UpdateAction()
    {
        return "Update";
    }

    public static string DeleteAction()
    {
        return "Delete";
    }

    public static string WaiveAction()
    {
        return "Waive";
    }

    public static string CancelAction()
    {
        return "Cancel";
    }
}
