using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Academic.Interfaces.Services;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetGroupSchedule(Guid groupId)
    {
        var schedule = await _scheduleService.GetGroupScheduleAsync(groupId);
        return Ok(schedule);
    }
}
