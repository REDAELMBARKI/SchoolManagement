using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Services;

namespace SchoolManagement.Backend.Http.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly GroupService _service;

    public GroupController(GroupService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GroupRequestDto dto)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] GroupRequestDto dto)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }



}
