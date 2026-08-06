using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Queries;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Application.Common.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly IWhatsAppMessageRepository _repository;
    private readonly IWhatsAppMessageQueryService _queryService;
    private readonly ICurrentUserContext _currentUserContext;

    public WhatsAppService(
        IWhatsAppMessageRepository repository,
        IWhatsAppMessageQueryService queryService,
        ICurrentUserContext currentUserContext
        )
    {
        _repository = repository;
        _queryService = queryService;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> QueueMessageAsync(
        string phoneNumber,
        string message,
        WhatsAppMessageType messageType,
        string? entityType = null,
        Guid? entityId = null,
        DateTime? scheduledFor = null)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        var whatsAppMessage = WhatsAppMessage.Create(
            branchId: branchId,
            phoneNumber: phoneNumber,
            message: message,
            messageType: messageType,
            entityType: entityType,
            entityId: entityId,
            scheduledFor: scheduledFor
        );

        var created = await _repository.AddAsync(whatsAppMessage);
       
        return created.Id;
    }

    public async Task<List<Guid>> QueueBulkMessagesAsync(
        List<string> phoneNumbers,
        string message,
        WhatsAppMessageType messageType,
        string? entityType = null)
    {
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        var messageIds = new List<Guid>();

        foreach (var phoneNumber in phoneNumbers)
        {
            var whatsAppMessage = WhatsAppMessage.Create(
                branchId: branchId,
                phoneNumber: phoneNumber,
                message: message,
                messageType: messageType,
                entityType: entityType
            );

            var created = await _repository.AddAsync(whatsAppMessage);
            messageIds.Add(created.Id);
        }

     
        return messageIds;
    }

    public async Task<WhatsAppMessage?> GetMessageStatusAsync(Guid messageId)
    {
        return await _repository.GetByIdAsync(messageId);
    }

    public async Task<List<WhatsAppMessage>> GetMessagesForEntityAsync(string entityType, Guid entityId)
    {
        return await _queryService.GetMessagesByEntityAsync(entityType, entityId);
    }

    public async Task RetryFailedMessageAsync(Guid messageId)
    {
        var message = await _repository.GetByIdAsync(messageId);
        if (message == null)
            throw new NotFoundException($"WhatsApp message with ID {messageId} not found.");

        if (!message.CanRetry())
            throw new DomainException("Message cannot be retried (either not failed or max retries reached).");

        message.ResetForRetry();
        await _repository.UpdateAsync(message);

        Console.WriteLine($"🔄 WhatsApp message retry scheduled: {messageId}");
    }
}
