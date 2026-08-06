using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Application.Common.Interfaces.Queries;

public interface IWhatsAppMessageQueryService : IEntityQuery<WhatsAppMessage>
{
    /// <summary>
    /// Get all WhatsApp messages for a specific entity
    /// </summary>
    Task<List<WhatsAppMessage>> GetMessagesByEntityAsync(string entityType, Guid entityId);
}
