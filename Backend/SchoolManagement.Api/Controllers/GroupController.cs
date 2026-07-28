using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _service;

    public GroupController(IGroupService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var groups = await _service.GetAllAsync();
        return Ok(groups);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var group = await _service.GetByIdAsync(id);
            if (group is null) return NotFound();
            return Ok(group);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Fetch error",
                detail: ex.Message
            );
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GroupRequestDto dto)
    {
        try
        {
            var command = new GroupCommand
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                Period = dto.Period,
                LevelId = dto.LevelId,
                SubjectId = dto.SubjectId
            };
            var group = await _service.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Create error",
                detail: ex.Message
            );
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] GroupRequestDto dto)
    {
        try
        {
            var command = new UpdateGroupCommand
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                Period = dto.Period,
                LevelId = dto.LevelId,
                SubjectId = dto.SubjectId
            };
            var updated = await _service.UpdateAsync(id, command);
            if (updated is null) return NotFound();
            return Ok(updated);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Update error",
                detail: ex.Message
            );
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: 500,
                title: "Delete error",
                detail: ex.Message
            );
        }
    }



}
