using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Core.Mappers;
using SchoolManagement.Application.Common.Mappers;
using SchoolManagement.Application.Common.Validators;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Enums;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Application.Common.Interfaces;

namespace SchoolManagement.Application.Common.Services;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _repository;
    private readonly IStudentRepository _studentRepository;
    private readonly MediaStorageValidator _validator;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;
    private readonly IDomainUserRepository _domainUserRepository;

    public MediaService(
        IMediaRepository repository,
        IStudentRepository studentRepository,
        IDomainUserRepository domainUserRepository ,
        MediaStorageValidator validator,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _domainUserRepository = domainUserRepository;
        _studentRepository = studentRepository;
        _validator = validator;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
    }

    public async Task<MediaResponseDto> Upload(IFormFile file, Guid ownerId, OwnerType ownerType, MediaCollection collection, MediaType mediaType)
    {
        // 1. Validate file (extension, MIME type, file size)
        _validator.ValidateFile(file, mediaType);

        // 2. Get branch context
        var branchId = _currentUserContext.BranchId;
        if (branchId == Guid.Empty)
            throw new DomainException("Branch context is missing.");

        // 3. Validate branch quota (if enabled)
        await _validator.ValidateBranchQuotaAsync(branchId, file.Length);

        // 4. Validate owner exists
        await ValidateOwnerExistsAsync(ownerId, ownerType);

        // 5. Store file to disk
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLower();
        string finalPath = Path.Combine(filePath, uniqueName);

        using (var stream = new FileStream(finalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 6. Create media entity
        var media = Media.Create(
            url: $"/uploads/{uniqueName}",
            mimeType: file.ContentType,
            size: file.Length,
            altText: null,
            width: null,  // TODO: Add ImageSharp for dimension extraction
            height: null,
            ownerType: ownerType,
            ownerId: ownerId,
            mediaType: mediaType,
            collection: collection,
            order: 0,
            isMain: false,
            storageProvider: "Local",
            branchId: branchId
        );

        var storedMedia = await _repository.Add(media);

        // 7. Audit log
        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Media",
            entityId: storedMedia.Id,
            branchId: branchId,
            newValues: new
            {
                storedMedia.Url,
                storedMedia.Size,
                storedMedia.MimeType,
                storedMedia.OwnerType,
                storedMedia.OwnerId,
                storedMedia.MediaType,
                storedMedia.Collection
            },
            message: $"Media uploaded for {ownerType} {ownerId}: {file.FileName} ({file.Length} bytes)");

        return MediaMapper.ToResponse(storedMedia);
    }

    /// <summary>
    /// Validates that the owner entity exists in the database.
    /// </summary>
    private async Task ValidateOwnerExistsAsync(Guid ownerId, OwnerType ownerType)
    {
        switch (ownerType)
        {
            case OwnerType.Student:
                var student = await _studentRepository.GetByIdAsync(ownerId);
                if (student == null)
                    throw new NotFoundException($"Student with ID {ownerId} not found.");
                break;

            case OwnerType.User:
                var user = await _domainUserRepository.GetByIdAsync(ownerId);
                if (user == null)
                    throw new NotFoundException($"User with ID {ownerId} not found.");
                break;

            case OwnerType.Teacher:
                // TODO: Add ITeacherRepository when Teacher entity is implemented
                // For now, skip validation for Teacher
                break;

            case OwnerType.Administrator:
                // TODO: Add administrator validation logic when Admin entity is implemented
                // For now, skip validation for Administrator
                break;

            default:
                throw new DomainException($"Unknown owner type: {ownerType}");
        }
    }
}
