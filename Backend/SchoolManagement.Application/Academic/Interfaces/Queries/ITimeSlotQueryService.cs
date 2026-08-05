using SchoolManagement.Domain.Academic.Entities;

namespace SchoolManagement.Application.Academic.Interfaces.Queries;

public interface ITimeSlotQueryService
{
    Task<TimeSlot?> GetByIdAsync(Guid id);
    Task<List<TimeSlot>> GetAllAsync();
    Task<TimeSlot?> GetByTimesAsync(TimeOnly startTime, TimeOnly endTime);
}
