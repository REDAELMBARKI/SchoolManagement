using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;
namespace SchoolManagement.Backend.Database.Factories;

public class RoomFactory : Factory<Room>
{
    public RoomFactory(AppDbContext context) : base(context)
    {
    }

    protected override Room Make()
    {
        return new Room
        {
            
        };
    }
}