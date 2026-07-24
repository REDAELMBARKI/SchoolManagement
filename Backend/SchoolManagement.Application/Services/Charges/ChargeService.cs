using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Application.Services.Charges;

public class ChargeService : IChargeService
{
    private readonly IChargeRepository _repository;

    public ChargeService(IChargeRepository repository )
    {
        _repository = repository;

    }

    public async Task<List<ChargeResponseDto>> GetAllAsync()
    {
         throw new NotImplementedException();
    }

    public async Task<ChargeResponseDto?> GetByIdAsync(Guid id)
    {
        var charge = await _repository.GetByIdAsync(id);
        if (charge == null) return null;
        return ChargeMapper.ToResponse(charge);
    }

    public async Task<ChargeResponseDto> CreateAsync(ChargeCommand chargeCommand)
    {
        var charge = ChargeMapper.ToDomain(chargeCommand);
        var createdCharge = await _repository.AddAsync(charge);
        return ChargeMapper.ToResponse(createdCharge);
    }

    public async Task<ChargeResponseDto> UpdateAsync(Guid id, UpdateChargeRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new NotFoundException($"No charge found with id {id}");
        }

        existing.UpdateAmount(dto.Amount);
        existing.UpdateDescription(dto.Description);
        existing.UpdateDueDate(dto.DueDate);

        var updated = await _repository.UpdateAsync(existing);
        return ChargeMapper.ToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}