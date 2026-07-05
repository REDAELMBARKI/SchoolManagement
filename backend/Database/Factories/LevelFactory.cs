using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;

namespace SchoolManagement.Backend.Database.Factories ; 
public class LevelFactory : Factory<Level>
{
    private int _order = 1;

    public LevelFactory(AppDbContext context) : base(context)
    {
    }

    protected override  Task<Level> Make()
    {
        var branches = Context.Branches.Select(b => b.Id).ToList();

        var level =   new Level{   
            BranchId = faker.PickRandom(branches) ,
            Name  = faker.PickRandom("A0" , "A1", "A2", "B1", "B2", "C1", "C2"),
            Order = _order++,
        };
        return Task.FromResult(level);
    }
}