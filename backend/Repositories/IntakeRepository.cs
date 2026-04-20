using _.Dtos.Responses;
using _.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Interfaces;
using SQLitePCL;

namespace SchoolManagement.Repositories;

public class IntakeRepository : Repository<Intake> ,  IIntakeRepository
{
     
    public IntakeRepository(AppDbContext context) : base(context) { }
    public async Task<List<Intake>> GetAllAsync()
    {
         return await Query()
               .Include(intake => intake.LeadSource)
               .Include(intake => intake.Opc)
               .Include(intake => intake.Gender)
               .Select(intake => new Intake{
                   FirstName  = intake.FirstName , 
                   LastName = intake.LastName ,
                })
               .ToListAsync() ;
        
    } 
    public async Task<Intake?> GetOneAsync(int id)
    {
         return await Query()
                        .Include(intake => intake.LeadSource)
                        .Include(intake => intake.Opc)
                        .Include(intake => intake.Gender)
                        .FirstAsync();
    } 

    public Task<bool> ExistsAsync(int id)
    {
        throw new NotImplementedException("sdjosjd");
        
    }

}