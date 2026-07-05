using Microsoft.EntityFrameworkCore;
using SchoolManagement.Backend.Database.Factories;
using SchoolManagement.Backend.Contexts ;
namespace SchoolManagement.Backend.Database.Seeders;

public class UserSeeder : Seeder
{
    private readonly UserFactory _userFactory;

    public UserSeeder(AppDbContext context) : base(context)
    {  
        _userFactory = new UserFactory(context);
    }

    public override async Task RunAsync()
    {
        if (await Context.Opcs.AnyAsync()) return;

        var opc = await _userFactory.MakeOpc();
        // make opc 
        await Context.Opcs.AddAsync(opc);

        // ,male commercial agent
        var ca = await _userFactory.MakeCa();
        await Context.CommercialAgents.AddAsync(ca);
        await Context.SaveChangesAsync();
    }

    
}