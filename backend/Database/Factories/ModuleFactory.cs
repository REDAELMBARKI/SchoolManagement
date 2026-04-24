using SchoolManagement.Backend.Models;
namespace SchoolManagement.Backend.Database.Factories;

public class ModuleFactory : Factory<Module>
{
    public ModuleFactory(AppDbContext context) : base(context)
    {
    }

    protected override Module Make()
    {
        return new Module
        {
            
        };
    }
}