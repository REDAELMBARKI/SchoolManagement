using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string EntityName { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty; // Create, Update, Delete
    public string? OldValues { get; private set; } // JSON
    public string? NewValues { get; private set; } // JSON
    public string? ChangedBy { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid BranchId { get; private set; }

    public virtual Branch Branch { get; private set; } = null!;

    private AuditLog() { }

    public static AuditLog Create(
        string entityName,
        Guid entityId,
        string action,
        string? oldValues,
        string? newValues,
        string? changedBy,
        Guid branchId)
    {
        return new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
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


    public static string CreateAction(){
        return "Create";
    }


    public static string UpdateAction(){
       return  "Update";
    }


    public static string DeleteAction(){
        return "Delete";
    }


}
