using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Domain.Common.Entities;

public class WhatsAppMessage : AggregateRoot
{
    public Guid BranchId { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public WhatsAppMessageType MessageType { get; private set; }
    public WhatsAppMessageStatus Status { get; private set; } = WhatsAppMessageStatus.Pending;
    public string? EntityType { get; private set; }
    public Guid? EntityId { get; private set; }
    public DateTime? ScheduledFor { get; private set; }
    public DateTime? SentAt { get; private set; }
    public int RetryCount { get; private set; } = 0;
    public string? ErrorMessage { get; private set; }

    // Navigation
    public virtual Branch Branch { get; private set; } = null!;

    private const int MaxRetries = 5;

    private WhatsAppMessage() { }

    public static WhatsAppMessage Create(
        Guid branchId,
        string phoneNumber,
        string message,
        WhatsAppMessageType messageType,
        string? entityType = null,
        Guid? entityId = null,
        DateTime? scheduledFor = null)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number cannot be empty.");
        if (string.IsNullOrWhiteSpace(message))
            throw new DomainException("Message cannot be empty.");

        return new WhatsAppMessage
        {
            BranchId = branchId,
            PhoneNumber = phoneNumber,
            Message = message,
            MessageType = messageType,
            EntityType = entityType,
            EntityId = entityId,
            ScheduledFor = scheduledFor,
            Status = WhatsAppMessageStatus.Pending
        };
    }

    public void MarkAsProcessing()
    {
        if (Status != WhatsAppMessageStatus.Pending)
            throw new DomainException("Only pending messages can be marked as processing.");
        Status = WhatsAppMessageStatus.Processing;
    }

    public void MarkAsSent()
    {
        if (Status != WhatsAppMessageStatus.Processing)
            throw new DomainException("Only processing messages can be marked as sent.");
        Status = WhatsAppMessageStatus.Sent;
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void MarkAsFailed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new DomainException("Error message cannot be empty.");

        Status = WhatsAppMessageStatus.Failed;
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    public bool CanRetry()
    {
        return Status == WhatsAppMessageStatus.Failed && RetryCount < MaxRetries;
    }

    public void ResetForRetry()
    {
        if (!CanRetry())
            throw new DomainException("Message cannot be retried (either not failed or max retries reached).");
        
        Status = WhatsAppMessageStatus.Pending;
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number cannot be empty.");
        PhoneNumber = phoneNumber;
    }

    public void UpdateMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new DomainException("Message cannot be empty.");
        Message = message;
    }
}
