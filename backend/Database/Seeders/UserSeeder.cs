using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;

public class UserSeeder : Seeder
{
    private readonly UserFactory _userFactory;

    public UserSeeder(AppDbContext context , IMapper mapper) : base(context)
    {  
        _userFactory = new UserFactory(context , mapper);
    }

    public override async Task RunAsync()
    {
        if (await Context.Genders.AnyAsync()) return;

        var opc = _userFactory.MakeOpc();
        // make opc 
        await Context.Opcs.AddAsync(opc);

        // ,male commercial agent
        var ca = _userFactory.MakeCa();
        await Context.CommercialAgents.AddAsync(ca);
        await Context.SaveChangesAsync();
    }

    
}