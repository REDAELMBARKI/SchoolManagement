using SchoolManagement.DTOs;
using SchoolManagement.Repositories;

namespace SchoolManagement.Services;

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

    public async Task AddIntakeAsync(IntakeDTO intake)
    {
        var entity = new Intake
        {
            Id = intake.Id,
            FirstName = intake.FirstName,
            LastName = intake.LastName,
            Phone = intake.Phone,
            Email = intake.Email,
            IntakeDate = intake.IntakeDate,
            OpcId = intake.OpcId,
            LeadSourceId = intake.LeadSourceId,
            GenderId = intake.GenderId
        };
        
        // await _repository.AddOneAsync(entity);
    }

 

}