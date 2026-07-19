using SchoolManagement.Application.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Stratigies.LeadSourceExistence
{
    public interface ILeadSourceChecker
    {
        LeadSourceType SourceType { get; }
        Task<bool> IsExistsChecker(Guid id );
    }
}
