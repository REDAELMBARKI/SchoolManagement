using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Repositories;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Services;

public class IntakeService
{
    private readonly IntakeRepository _repository;

    public IntakeService(IntakeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Intake>> GetAllIntakesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Intake> GetIntakeByIdAsync(int id)
    {
        return await _repository.GetOneAsync(id);
    }

    public async Task AddIntakeAsync(IntakeDto intakeDto)
    {   

        var entity = new Intake
        {
            FirstName = intakeDto.FirstName,
            LastName = intakeDto.LastName,
            Phone = intakeDto.Phone,
            Email = intakeDto.Email,
            IntakeDate = intakeDto.IntakeDate,
            LeadSourceId = intakeDto.LeadSourceId,
            GenderId = intakeDto.GenderId
        };
        
        await _repository.AddAsync(entity);
    }

 

}