using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Application.Services;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Services;

namespace SchoolManagement.Api.Controllers;

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
