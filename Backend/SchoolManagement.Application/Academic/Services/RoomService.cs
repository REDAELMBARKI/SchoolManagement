using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Academic.Mappers;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Application.Academic.Interfaces.Queries;


namespace SchoolManagement.Application.Academic.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _repository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuditLogService _auditLogService;
    private readonly IRoomQueryService _query;

    public RoomService(
        IRoomRepository repository,
        ICurrentUserContext currentUserContext,
        IAuditLogService auditLogService,
        IRoomQueryService query)
    {
        _repository = repository;
        _currentUserContext = currentUserContext;
        _auditLogService = auditLogService;
        _query = query;
    }

    public async Task<List<RoomResponseDto>> GetAllAsync()
    {
        var rooms = await _query.GetAllAsync();
        return rooms.Select(RoomMapper.ToResponse).ToList();
    }

    public async Task<RoomResponseDto> GetByIdAsync(Guid id)
    {
        var room = await _query.GetByIdAsync(id);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {id} not found.");
        }
        return RoomMapper.ToResponse(room);
    }

    public async Task<RoomResponseDto> CreateAsync(RoomCommand command)
    {
        var room = RoomMapper.ToDomain(command, _currentUserContext.BranchId);
        var created = await _repository.AddAsync(room);

        await _auditLogService.StoreAsync(
            action: AuditLog.CreateAction(),
            entityName: "Room",
            entityId: created.Id,
            branchId: _currentUserContext.BranchId,
            newValues: CreateAuditSnapshot(created));

        return RoomMapper.ToResponse(created);
    }

    public async Task<RoomResponseDto> UpdateAsync(Guid id, UpdateRoomCommand command)
    {
        var room = await _repository.GetByIdAsync(id);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(room);

        room.UpdateName(command.Name);
        room.UpdateCapacity(command.Capacity);
        room.UpdateFloor(command.Floor);
        room.UpdateDescription(command.Description);

        var updated = await _repository.UpdateAsync(room);

        await _auditLogService.StoreAsync(
            action: AuditLog.UpdateAction(),
            entityName: "Room",
            entityId: updated.Id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues,
            newValues: CreateAuditSnapshot(updated));

        return RoomMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var room = await _repository.GetByIdAsync(id);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {id} not found.");
        }

        var oldValues = CreateAuditSnapshot(room);

        await _repository.DeleteAsync(id);

        await _auditLogService.StoreAsync(
            action: AuditLog.DeleteAction(),
            entityName: "Room",
            entityId: id,
            branchId: _currentUserContext.BranchId,
            oldValues: oldValues);
    }

    private static object CreateAuditSnapshot(Domain.Academic.Entities.Room room)
    {
        return new
        {
            room.Id,
            room.Name,
            room.Capacity,
            room.Floor,
            room.Description,
            room.BranchId
        };
    }
}
