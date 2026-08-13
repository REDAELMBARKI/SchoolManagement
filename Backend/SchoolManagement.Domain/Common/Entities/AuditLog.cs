namespace SchoolManagement.Domain.Common.Entities;

public class AuditLog : BaseEntity
{
    public const string SeverityInfo = "Info";
    public const string SeverityWarning = "Warning";
    public const string SeverityHigh = "High";
    public const string SeverityCritical = "Critical";

    public const string CategoryBusiness = "Business";
    public const string CategorySecurity = "Security";
    public const string CategoryFinancial = "Financial";
    
    public string EntityName { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty; // Create, Update, Delete, Waive, etc.
    public string? Message { get; private set; }
    public string? OldValues { get; private set; } // JSON
    public string? NewValues { get; private set; } // JSON
    public string? ChangedBy { get; private set; }
    public string? HasRole { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid BranchId { get; private set; } // Non-nullable: Use SYSTEM_BRANCH_ID for global actions
    public string? AdditionalData { get; private set; } // JSON for any additional data
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Severity { get; private set; } // Info, Warning, High, Critical
    public string? Category { get; private set; } // Business, Security, Financial
    public virtual Branch? Branch { get; private set; } // Nullable navigation property (for SYSTEM_BRANCH_ID which won't have actual Branch)

    private AuditLog() { }

    public static AuditLog Create(
        string entityName,
        Guid entityId,
        string action,
        string? oldValues,
        string? newValues,
        string? changedBy,
        string? hasRole,
        Guid branchId,
        string? additionalData,
        string? message = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? severity = null,
        string? category = null)
    {
        var effectiveBranchId = branchId == Guid.Empty ? Branch.SYSTEM_BRANCH_ID : branchId;

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
            HasRole = hasRole,
            ChangedAt = DateTime.UtcNow,
            BranchId = effectiveBranchId, // Use effective branch ID with fallback
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Severity = severity ?? "Info",
            Category = category ?? "Business"
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

    public static string FailedLoginAction()
    {
        return "FailedLoginAttempt";
    }

    public static string AccountLockedAction()
    {
        return "AccountLocked";
    }

    public static string RoleChangedAction()
    {
        return "RoleChanged";
    }

    public static string UnauthorizedBranchAccessAction()
    {
        return "UnauthorizedBranchAccess";
    }

    public static string TokenBlacklistedAction()
    {
        return "TokenBlacklisted";
    }

    public static string PasswordResetAction()
    {
        return "PasswordReset";
    }
}
