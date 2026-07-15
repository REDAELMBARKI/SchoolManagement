using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Queries.Common;
using SchoolManagement.Domain.Interfaces.Repositories.Common;

namespace SchoolManagement.Domain.Interfaces.Repositories;

public interface ICommercialAgentRepository : IRepository<CommercialAgent> , IQuery<CommercialAgent>
{

}