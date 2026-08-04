using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Academic.Interfaces.Services;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/schedules")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    /// <summary>
    /// Create multiple schedules for a group (bulk creation)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateSchedules([FromBody] CreateSchedulesRequestDto request)
    {
        try
        {
            var result = await _scheduleService.CreateSchedulesAsync(request);
            return Ok(new { success = result, message = "Schedules created successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get schedule for a specific group, grouped by day
    /// </summary>
    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetGroupSchedule(Guid groupId)
    {
        try
        {
            var schedule = await _scheduleService.GetGroupScheduleAsync(groupId);
            return Ok(schedule);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a specific schedule
    /// </summary>
    [HttpPut("{scheduleId}")]
    public async Task<IActionResult> UpdateSchedule(Guid scheduleId, [FromBody] UpdateScheduleRequestDto request)
    {
        try
        {
            var result = await _scheduleService.UpdateScheduleAsync(scheduleId, request);
            return Ok(new { success = result, message = "Schedule updated successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a specific schedule (soft delete)
    /// </summary>
    [HttpDelete("{scheduleId}")]
    public async Task<IActionResult> DeleteSchedule(Guid scheduleId)
    {
        try
        {
            var result = await _scheduleService.DeleteScheduleAsync(scheduleId);
            return Ok(new { success = result, message = "Schedule deleted successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Check room availability for AJAX validation (real-time feedback)
    /// </summary>
    [HttpGet("check-room-availability")]
    public async Task<IActionResult> CheckRoomAvailability(
        [FromQuery] Guid roomId,
        [FromQuery] Guid dayId,
        [FromQuery] TimeOnly startTime,
        [FromQuery] TimeOnly endTime,
        [FromQuery] Guid? excludeScheduleId = null)
    {
        var availability = await _scheduleService.CheckRoomAvailabilityAsync(
            roomId, dayId, startTime, endTime, excludeScheduleId);
        return Ok(availability);
    }

    /// <summary>
    /// Check teacher availability for AJAX validation (real-time feedback)
    /// </summary>
    [HttpGet("check-teacher-availability")]
    public async Task<IActionResult> CheckTeacherAvailability(
        [FromQuery] Guid teacherId,
        [FromQuery] Guid dayId,
        [FromQuery] TimeOnly startTime,
        [FromQuery] TimeOnly endTime,
        [FromQuery] Guid? excludeScheduleId = null)
    {
        var availability = await _scheduleService.CheckTeacherAvailabilityAsync(
            teacherId, dayId, startTime, endTime, excludeScheduleId);
        return Ok(availability);
    }
}
