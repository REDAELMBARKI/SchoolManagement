using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Backend.Models;

namespace SchoolManagement.Backend.Database.Factories ; 
public class LevelFactory : Factory<Level>
{
    private int _order = 1;

    public LevelFactory(AppDbContext context) : base(context)
    {
    }

    protected override Level Make()
    {
        var branches = Context.Branches.Select(b => b.Id).ToList();

       return  new Level{   
            BranchId = faker.PickRandom(branches) ,
            Name  = faker.PickRandom("A0" , "A1", "A2", "B1", "B2", "C1", "C2"),
            Order = _order++,
        };
    }
}