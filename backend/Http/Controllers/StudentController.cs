using _.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.DTOs;
using SchoolManagement.Http.Requests;
using SchoolManagement.Models;
using SchoolManagement.Repositories;
using SchoolManagement.Services;

namespace SchoolManagement.Http.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class StudentController : ControllerBase
{
    // private readonly StudentService _service;

    // public StudentController(StudentService service  )
    // {
    //     _service = service;
        
    // }

    // [HttpGet]
    // public IActionResult GetAll()
    // {  
    //     Console.WriteLine("hellow words") ;
    //     return Ok(new {message = "success"});
    // }

    // [HttpGet("{id}")]
    //  public async Task<IActionResult> GetById(int id)
    // {
    //     var Student = await _service.getStudentById(id);
    //     return Ok(Student);
    // }


    // [HttpPost]
    // public async Task<IActionResult> Create([FromBody] StudentRequest request , StudentDTO dto)
    // {   
    //     StudentDTO _dto = dto.FromRequest(request);
    //     StudentResponseDto resDto = await _service.CreateStudent(_dto);

    //     return Created( $"api/students/{resDto.Id}", resDto);
    // }


   
}