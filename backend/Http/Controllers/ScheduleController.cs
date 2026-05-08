using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Backend.Services;

namespace SchoolManagement.Backend.Http.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ScheduleService _scheduleService;

    public ScheduleController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetGroupSchedule(int groupId)
    {
        var schedule = await _scheduleService.GetGroupScheduleAsync(groupId);
        return Ok(schedule);
    }
}
