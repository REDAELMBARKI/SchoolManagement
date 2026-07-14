using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Data ;
namespace SchoolManagement.Infrastructure.Data.Factories;

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