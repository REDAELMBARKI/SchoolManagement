using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LevelController : ControllerBase
{
    private readonly ILevelService _levelService;

    public LevelController(ILevelService levelService)
    {
        _levelService = levelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var levels = await _levelService.GetAllAsync();
        return Ok(levels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var level = await _levelService.GetByIdAsync(id);
            return Ok(level);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LevelCommand command)
    {
        try
        {
            var level = await _levelService.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = level.Id }, level);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLevelCommand command)
    {
        try
        {
            var level = await _levelService.UpdateAsync(id, command);
            return Ok(level);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _levelService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
