using SchoolManagement.Backend.Entities;
using SchoolManagement.Backend.Data ;
namespace SchoolManagement.Backend.Data.Factories;

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