using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using SchoolManagement.Domain.Interfaces.Repositories.Common;
namespace SchoolManagement.Domain.Interfaces.Repositories;

public interface IGenderRepository : IRepository<Gender> , IQuery<Gender>
{
    
}