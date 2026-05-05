using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Interfaces;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Mappers;
using SchoolManagement.Backend.Dtos;
using SchoolManagement.Backend.Exceptions;

namespace SchoolManagement.Backend.Repositories;

public class IntakeRepository : Repository<Intake> 
{
     
     // read 
    public IntakeRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<List<IntakeResponseDto>> GetAllAsync()
    {

         var intakes =  await Query()
                .Include(i => i.Gender)
                .Include(i => i.LeadSource)
                   .ThenInclude(ld => ld!.Ad)
                     .ThenInclude(ad => ad!.Platform)
                .Include(i => i.LeadSource)
                   .ThenInclude(ld => ld!.Opc)
                .Include(i => i.Subject)
                .Include(i => i.CommercialAgent)
                .Include(i => i.Branch)
                .Include(i => i.ConvertedToStudent)
               .ToListAsync() ;

         return intakes
               .Select(intake => new IntakeResponseDto{
                   Id = intake.Id ,
                   IntakeDate = intake.IntakeDate ,
                   Phone = intake.Phone ,
                   FirstName  = intake.FirstName , 
                   Email = intake.Email ,
                   LastName = intake.LastName ,
                   Slug = intake.Slug ,
                   CreatedAt = intake.CreatedAt ,
                   DateOfBirth = intake.DateOfBirth ,
                   FollowUpDate = intake.FollowUpDate ,
                   Notes = intake.Notes ,
                   Status = intake.Status ,
                   IsIndependent = intake.IsIndependent ,
                   TotalFees = intake.TotalFees ,
                   AmountPaid = intake.AmountPaid ,
                   Subject = IntakeMapper.MapSubject(intake.Subject) ,
                   Gender = IntakeMapper.MapGender(intake.Gender) ,
                   CommercialAgent =  IntakeMapper.MapCommercialAgent(intake.CommercialAgent) ,
                   Branch =  IntakeMapper.MapBranch(intake.Branch) ,
                   LeadSource = IntakeMapper.MapLeadSource(intake.LeadSource)
                   
         })
         .ToList() ;
        
    } 
    public async Task<IntakeResponseDto?> GetOneAsync(int id)
    {
        var intake = await Query()
                .Include(i => i.Gender)
                .Include(i => i.LeadSource)
                .ThenInclude(ld => ld!.Ad)
                    .ThenInclude(ad => ad!.Platform)
                .Include(i => i.LeadSource)
                .ThenInclude(ld => ld!.Opc)
                .Include(i => i.Subject)
                .Include(i => i.CommercialAgent)
                .Include(i => i.Branch)
                .Include(i => i.ConvertedToStudent)
                .FirstOrDefaultAsync(i => i.Id == id);

        if(intake is null) throw new NotFoundException($"no intake found with id {id}");

        return new IntakeResponseDto
        {
            Id = intake.Id,
            IntakeDate = intake.IntakeDate,
            Phone = intake.Phone,
            FirstName = intake.FirstName,
            Email = intake.Email,
            LastName = intake.LastName,
            Slug = intake.Slug,
            CreatedAt = intake.CreatedAt,
            DateOfBirth = intake.DateOfBirth,
            FollowUpDate = intake.FollowUpDate,
            Notes = intake.Notes,
            Status = intake.Status,
            IsIndependent = intake.IsIndependent,
            TotalFees = intake.TotalFees,
            AmountPaid = intake.AmountPaid,
            Subject = IntakeMapper.MapSubject(intake.Subject),
            Gender = IntakeMapper.MapGender(intake.Gender),
            CommercialAgent = IntakeMapper.MapCommercialAgent(intake.CommercialAgent),
            Branch = IntakeMapper.MapBranch(intake.Branch),
            LeadSource = IntakeMapper.MapLeadSource(intake.LeadSource)
        };
    }
   


    //  write 
    public async Task<IntakeResponseDto> AddAsync(Intake intake)
    {
         await Context.AddAsync(intake);
         await Context.SaveChangesAsync();
         return (await  this.GetOneAsync(intake.Id))!;
    }


    public async Task UpdateAsync(int id , Intake intake)
    {   
        Intake? dbIntake = await Context.Intakes.FindAsync(id);
        if(dbIntake is null ) throw new NotFoundException($"no intake found with id {id}") ;
        intake.Id = dbIntake.Id ;
        Context.Entry(dbIntake).CurrentValues.SetValues(intake) ;
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Intake? dbIntake = await Context.Intakes.FindAsync(id);
        if(dbIntake is null ) throw new NotFoundException($"no intake found with id {id}")  ;
        dbIntake.DeletedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync();
    }
}