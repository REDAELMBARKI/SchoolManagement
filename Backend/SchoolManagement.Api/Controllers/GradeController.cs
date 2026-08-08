using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Academic.Dtos.Commands;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/grades")]
public class GradeController : ControllerBase
{
    private readonly IGradeService _service;

    public GradeController(IGradeService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGradeRequestDto request)
    {
        try
        {
            var command = new GradeCommand
            {
                EvaluationType = request.EvaluationType,
                Score = request.Score,
                MaxScore = request.MaxScore,
                EvaluationDate = request.EvaluationDate,
                Comment = request.Comment,
                StudentId = request.StudentId,
                GroupTeacherId = request.GroupTeacherId
            };

            var result = await _service.CreateAsync(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGradeRequestDto request)
    {
        try
        {
            var command = new UpdateGradeCommand
            {
                EvaluationType = request.EvaluationType,
                Score = request.Score,
                MaxScore = request.MaxScore,
                Comment = request.Comment
            };

            var result = await _service.UpdateAsync(id, command);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentId)
    {
        var result = await _service.GetByStudentAsync(studentId);
        return Ok(result);
    }

    [HttpGet("group-teacher/{groupTeacherId:guid}")]
    public async Task<IActionResult> GetByGroupTeacher(Guid groupTeacherId)
    {
        var result = await _service.GetByGroupTeacherAsync(groupTeacherId);
        return Ok(result);
    }
}
