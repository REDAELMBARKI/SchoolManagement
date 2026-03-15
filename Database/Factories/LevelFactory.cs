using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Models;

namespace SchoolManagement.Database.Factories ; 
public class LevelFactory : Factory<Level>
{
    private int _order = 1;

    protected override Level Make() => new Level
    {
        Name  = faker.PickRandom("A1", "A2", "B1", "B2", "C1", "C2", "D1", "D2"),
        Order = _order++,
    };
}