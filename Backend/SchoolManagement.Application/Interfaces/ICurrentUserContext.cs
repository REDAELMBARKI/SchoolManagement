using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Interfaces
{
    public interface ICurrentUserContext
    {

        Guid BranchId { get; }
        
    }
}
