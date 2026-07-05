using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Contexts ;
namespace SchoolManagement.Backend.Database.Factories;

public class RoomFactory : Factory<Room>
{
    public RoomFactory(AppDbContext context) : base(context)
    {
    }

    protected override  Task<Room> Make()
    {
        var room = new Room
        {
            
        };
        return Task.FromResult(room);
    }
}