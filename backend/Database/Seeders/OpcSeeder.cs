using SchoolManagement.Backend.Database.Factories;
namespace SchoolManagement.Backend.Database.Seeders;

public class OpcSeeder : Seeder
{
    private readonly UserFactory _userFactory;

    public OpcSeeder(UserFactory userFactory , AppDbContext context) : base(context)
    {  
        _userFactory = userFactory;
    }

    public override async Task RunAsync()
    {
        var opc = _userFactory.MakeOpc();
        await Context.Opcs.AddAsync(opc);
        await Context.SaveChangesAsync();
    }
}