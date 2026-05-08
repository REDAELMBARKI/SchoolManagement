using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Exceptions;
using SchoolManagement.Backend.Services;

namespace SchoolManagement.Backend.Http.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentService _enrollmentService;

    public EnrollmentController(EnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enrollments = await _enrollmentService.GetAllAsync();
        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);
            return Ok(enrollment);
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
    public async Task<IActionResult> Create([FromBody] EnrollmentRequestDto dto)
    {
        try
        {
            var enrollment = await _enrollmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
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
    public async Task<IActionResult> Update(int id, [FromBody] EnrollmentRequestDto dto)
    {
        try
        {
            await _enrollmentService.UpdateAsync(id, dto);
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
            await _enrollmentService.DeleteAsync(id);
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

    // TODO: Additional endpoints for you to implement:
    // - GET api/enrollments/student/{studentId} - Get student's enrollments
    // - GET api/enrollments/group/{groupId} - Get group enrollments
    // - POST api/enrollments/{id}/drop - Drop enrollment (change status)
    // - POST api/enrollments/{id}/payment - Process payment
}
