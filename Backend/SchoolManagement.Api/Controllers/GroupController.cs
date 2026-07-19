using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Interfaces.Services;

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
        // TODO: Implement
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
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
    public async Task<IActionResult> Update(Guid id, [FromBody] GroupRequestDto dto)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }



}
