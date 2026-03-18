using Microsoft.AspNetCore.Mvc;
using SchoolManagement.DTOs;
using SchoolManagement.Http.Requests;
using SchoolManagement.Models;
using SchoolManagement.Repositories;
using SchoolManagement.Services;

namespace SchoolManagement.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class StudentController : ControllerBase
{
    private readonly StudentService _service;

    public StudentController(StudentService service  )
    {
        _service = service;
        
    }

    [HttpGet]
    public IActionResult GetAll()
    {  
        Console.WriteLine("hellow words") ;
        return Ok(new {message = "success"});
    }

    [HttpGet("{id}")]
     public IActionResult getById(int id)
    {
        Student student =  _service.getStudentById(id)
        return Ok(user)
    }


    [HttpPost]
    public IActionResult Create([FromBody] CreateStudentRequest request , StudentDTO dto)
    {   
        StudentDTO _dto = dto.FromRequest(request);
        _service.createStudent(_dto);
        return Ok(new {message = $"the Student has been created with Id "});
    }


   
}