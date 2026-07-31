using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Application.Academic.Services;
using SchoolManagement.Application.Core.Services;
using SchoolManagement.Domain.Common.Exceptions;
using SchoolManagement.Application.Academic.Interfaces.Services;
using SchoolManagement.Application.Core.Interfaces.Services;
using SchoolManagement.Application.Common.Interfaces.Services;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;

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
