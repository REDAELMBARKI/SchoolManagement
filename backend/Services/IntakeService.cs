using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Repositories;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Dtos.Responses;
using AutoMapper;
using SchoolManagement.Backend.Mappers;

namespace SchoolManagement.Backend.Services;

public class IntakeService
{
    private readonly IntakeRepository _repository;
    private readonly IMapper _autoMapper ;
    public IntakeService(IntakeRepository repository , IMapper mapper)
    {
        _repository = repository;
        _autoMapper = mapper ;
    }

    public async Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync()
    {
        return await _repository.GetAllAsync();
        
    }

    public async Task<IntakeResponseDto?> GetIntakeByIdAsync(int id)
    {
        return await _repository.GetOneAsync(id);
    }

    public async Task<IntakeResponseDto> AddIntakeAsync(IntakeDto intakeDto)
    {   

        var entity = new Intake
        {
            FirstName = intakeDto.FirstName,
            LastName = intakeDto.LastName,
            Phone = intakeDto.Phone,
            Email = intakeDto.Email,
            DateOfBirth = intakeDto.DateOfBirth,
            IntakeDate = intakeDto.IntakeDate,
            Status = intakeDto.Status,
            FollowUpDate = intakeDto.FollowUpDate,
            Notes = intakeDto.Notes,
            GenderId = intakeDto.GenderId,
            LeadSourceId = intakeDto.LeadSourceId,
            ProgramId = intakeDto.ProgramId,
            BranchId = intakeDto.BranchId,
            CommercialAgentId = intakeDto.CommercialAgentId,
        };
        
        var newEntity = await _repository.AddAsync(entity);
        return newEntity ; 
    }


    public async Task UpdateAsync(int id , IntakeDto intakeDto)
    {
          var intake = IntakeMapper.ToEntity(intakeDto);
          await _repository.UpdateAsync(id , intake);
    }


    public async Task DeleteIntakeAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
 

}