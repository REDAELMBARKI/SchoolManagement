using SchoolManagement.Application.Academic.Dtos.Requests;
using SchoolManagement.Application.Core.Dtos.Requests;
using SchoolManagement.Application.Common.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Core.Strategies
{
    public interface ILeadSourceChecker
    {
        LeadSourceType SourceType { get; }
        Task<bool> IsExistsChecker(Guid id );
    }
}
