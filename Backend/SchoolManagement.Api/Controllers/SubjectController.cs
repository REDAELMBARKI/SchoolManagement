using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subjects = await _subjectService.GetAllAsync();
        return Ok(subjects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var subject = await _subjectService.GetByIdAsync(id);
            return Ok(subject);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SubjectRequestDto request)
    {
        try
        {
            var command = new SubjectCommand
            {
                Name = request.Name,
                Description = request.Description
            };
            var subject = await _subjectService.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubjectRequestDto request)
    {
        try
        {
            var command = new UpdateSubjectCommand
            {
                Name = request.Name,
                Description = request.Description
            };
            var subject = await _subjectService.UpdateAsync(id, command);
            return Ok(subject);
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
            await _subjectService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
