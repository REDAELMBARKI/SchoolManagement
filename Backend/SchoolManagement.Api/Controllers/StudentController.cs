using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Dtos.Requests;
 using SchoolManagement.Domain.Events;
 using SchoolManagement.Domain.Exceptions; 
 using SchoolManagement.Application.Services; 
 namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly StudentService _studentService;
  
    public StudentController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var student = await _studentService.GetByIdAsync(id);
            return Ok(student);
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
    public async Task<IActionResult> Create([FromBody] StudentRequestDto dto)
    {
        try
        {
            var student = await _studentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
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
    public async Task<IActionResult> Update(int id, [FromBody] StudentRequestDto dto)
    {
        try
        {
            await _studentService.UpdateAsync(id, dto);
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
                title: "Update error",
                detail: ex.Message
            );
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _studentService.DeleteAsync(id);
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
