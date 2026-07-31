using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Core.Interfaces.Queries;

public interface IOpcQueryService : IEntityQuery<Opc>
{
}
