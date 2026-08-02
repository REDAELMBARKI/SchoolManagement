using SchoolManagement.Application.Academic.Dtos.Responses;
using SchoolManagement.Application.Core.Dtos.Responses;
using SchoolManagement.Application.Common.Dtos.Responses;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Application.Common.Interfaces.Queries;

namespace SchoolManagement.Application.Common.Interfaces.Queries;

public interface IGenderQueryService : IEntityQuery<Gender>
{
    Task<List<GenderResponseDto>> GetAllResponsesAsync();
    Task<GenderResponseDto?> GetResponseByIdAsync(Guid id);
}
