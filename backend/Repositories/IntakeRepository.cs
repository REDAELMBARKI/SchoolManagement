using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Interfaces;
using SQLitePCL;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Repositories;

public class IntakeRepository : Repository<Intake> ,  IIntakeRepository
{
     
     // read 
    public IntakeRepository(AppDbContext context) : base(context) { }
    public async Task<List<Intake>> GetAllAsync()
    {
         return await Query()
               .Include(intake => intake.LeadSource)
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
                        .Include(intake => intake.Gender)
                        .FirstAsync();
    } 
    public Task<bool> ExistsAsync(int id)
    {
        throw new NotImplementedException("sdjosjd");
        
    }

    //  write 
    public async Task<Intake> AddAsync(Intake entity)
    {
         await Context.AddAsync(entity);
         await Context.SaveChangesAsync();
         return entity;
    }


    public async Task<Intake?> UpdateAsync(int id , Intake intake)
    {
        Intake? dbIntake = await Context.Intakes.FindAsync(id);
        if(dbIntake is null ) return null ;
        return dbIntake ;
    }

    public async Task DeleteAsync(int id)
    {
        Intake? dbIntake = await Context.Intakes.FindAsync(id);
        if(dbIntake is null ) return  ;
        Context.Intakes.Remove(dbIntake);
        await Context.SaveChangesAsync();
    }
}