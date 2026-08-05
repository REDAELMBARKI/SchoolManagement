using SchoolManagement.Application.Core.Dtos.Requests;

namespace SchoolManagement.Application.Core.Strategies
{
    public interface ILeadSourceChecker
    {
        LeadSourceType SourceType { get; }
        Task<bool> IsExistsChecker(Guid id );
    }
}
