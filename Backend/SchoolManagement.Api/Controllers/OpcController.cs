using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Application.Services;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Services;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Api.Controllers;


[ApiController]
[Route("api/opcs")]
public class OpcController : ControllerBase
{
 
   public readonly AppDbContext _context ;

   public OpcController(AppDbContext context)
   {
      _context = context;
   }

 
  [HttpGet]
   public async Task<IActionResult> GetAll()
   {
      var opcs = await _context.Opcs.ToListAsync();
      return Ok(opcs);
   }


   
    [HttpPost]

   public  async Task<IActionResult> AddOpc()
   {
       throw new NotImplementedException();
    }

}
