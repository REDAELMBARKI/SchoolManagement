using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Interfaces.Services;

public interface IWhatsAppService
{
    /// <summary>
    /// Queue a WhatsApp message to be sent by the Node.js worker
    /// </summary>
    Task<Guid> QueueMessageAsync(
        string phoneNumber,
        string message,
        WhatsAppMessageType messageType,
        string? entityType = null,
        Guid? entityId = null,
        DateTime? scheduledFor = null);

    /// <summary>
    /// Queue multiple messages (bulk)
    /// </summary>
    Task<List<Guid>> QueueBulkMessagesAsync(
        List<string> phoneNumbers,
        string message,
        WhatsAppMessageType messageType,
        string? entityType = null);

    /// <summary>
    /// Get message status by ID
    /// </summary>
    Task<WhatsAppMessage?> GetMessageStatusAsync(Guid messageId);

    /// <summary>
    /// Get all messages for an entity
    /// </summary>
    Task<List<WhatsAppMessage>> GetMessagesForEntityAsync(string entityType, Guid entityId);

    /// <summary>
    /// Retry a failed message
    /// </summary>
    Task RetryFailedMessageAsync(Guid messageId);
}
