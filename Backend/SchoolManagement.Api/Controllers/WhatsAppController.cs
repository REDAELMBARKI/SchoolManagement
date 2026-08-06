using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Entities;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController : ControllerBase
{
    private readonly IWhatsAppService _whatsAppService;

    public WhatsAppController(IWhatsAppService whatsAppService)
    {
        _whatsAppService = whatsAppService;
    }

    /// <summary>
    /// Queue a single WhatsApp message
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendWhatsAppRequest request)
    {
        var messageId = await _whatsAppService.QueueMessageAsync(
            phoneNumber: request.PhoneNumber,
            message: request.Message,
            messageType: request.MessageType,
            entityType: request.EntityType,
            entityId: request.EntityId,
            scheduledFor: request.ScheduledFor
        );

        return Ok(new { messageId, status = "queued" });
    }

    /// <summary>
    /// Queue bulk WhatsApp messages
    /// </summary>
    [HttpPost("send-bulk")]
    public async Task<IActionResult> SendBulkMessages([FromBody] SendBulkWhatsAppRequest request)
    {
        var messageIds = await _whatsAppService.QueueBulkMessagesAsync(
            phoneNumbers: request.PhoneNumbers,
            message: request.Message,
            messageType: request.MessageType,
            entityType: request.EntityType
        );

        return Ok(new { messageIds, count = messageIds.Count, status = "queued" });
    }

    /// <summary>
    /// Get message status
    /// </summary>
    [HttpGet("status/{messageId}")]
    public async Task<IActionResult> GetMessageStatus(Guid messageId)
    {
        var message = await _whatsAppService.GetMessageStatusAsync(messageId);
        
        if (message == null)
            return NotFound();

        return Ok(new
        {
            message.Id,
            message.PhoneNumber,
            message.Status,
            message.MessageType,
            message.SentAt,
            message.ErrorMessage,
            message.RetryCount,
            message.CreatedAt
        });
    }

    /// <summary>
    /// Get all messages for an entity (e.g., all messages for an invoice)
    /// </summary>
    [HttpGet("entity/{entityType}/{entityId}")]
    public async Task<IActionResult> GetMessagesForEntity(string entityType, Guid entityId)
    {
        var messages = await _whatsAppService.GetMessagesForEntityAsync(entityType, entityId);

        return Ok(messages.Select(m => new
        {
            m.Id,
            m.PhoneNumber,
            m.Status,
            m.MessageType,
            m.SentAt,
            m.ErrorMessage,
            m.RetryCount,
            m.CreatedAt
        }));
    }

    /// <summary>
    /// Retry a failed message
    /// </summary>
    [HttpPost("retry/{messageId}")]
    public async Task<IActionResult> RetryMessage(Guid messageId)
    {
        await _whatsAppService.RetryFailedMessageAsync(messageId);
        return Ok(new { messageId, status = "retry_scheduled" });
    }
}

public class SendWhatsAppRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public WhatsAppMessageType MessageType { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public DateTime? ScheduledFor { get; set; }
}

public class SendBulkWhatsAppRequest
{
    public List<string> PhoneNumbers { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public WhatsAppMessageType MessageType { get; set; }
    public string? EntityType { get; set; }
}
