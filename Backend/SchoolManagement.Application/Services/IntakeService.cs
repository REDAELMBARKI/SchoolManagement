using AutoMapper;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Repositories;

namespace SchoolManagement.Application.Services;

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
        entity.Slug = await  CustomSluger.Slug(slug => _repository.IsExistBySlug(slug) , entity.FirstName , entity.LastName);
        var newEntity = await _repository.AddAsync(entity);
        return newEntity ;
    }

   
    
    public async Task UpdateAsync(int id , IntakeRequestDto intakeDto)
    {
          Intake intake = IntakeMapper.ToEntity(intakeDto);
          
          intake.Slug =  await CustomSluger.Slug( slug => _repository.IsExistBySlug(slug) , intake.FirstName, intake.LastName);
          intake.UpdatedAt = DateTime.Now ;
          await _repository.UpdateAsync(id , intake);
    }


    public async Task DeleteIntakeAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
 

}