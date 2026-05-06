using AutoMapper;
using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Mappers;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Repositories;
using SchoolManagement.Backend.Utils;

namespace SchoolManagement.Backend.Services;

public class IntakeService
{
    private readonly IntakeRepository _repository;
    public IntakeService(IntakeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<IntakeResponseDto>> GetAllIntakesAsync()
    {
        return await _repository.GetAllAsync();
        
    }

    public async Task<IntakeResponseDto?> GetIntakeByIdAsync(int id)
    {
        return await _repository.GetOneAsync(id);
    }

    public async Task<IntakeResponseDto> AddIntakeAsync(IntakeRequestDto intakeDto)
    {
        Intake entity = IntakeMapper.ToEntity(intakeDto);
        entity.Slug = CustomSluger.Slug(entity.FirstName , entity.LastName);
        var newEntity = await _repository.AddAsync(entity);
        return newEntity ;
    }


    public async Task UpdateAsync(int id , IntakeRequestDto intakeDto)
    {
          Intake intake = IntakeMapper.ToEntity(intakeDto);
          intake.Slug = CustomSluger.Slug(intake.FirstName, intake.LastName);
          intake.UpdatedAt = DateTime.Now ;
          await _repository.UpdateAsync(id , intake);
    }


    public async Task DeleteIntakeAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
 

}