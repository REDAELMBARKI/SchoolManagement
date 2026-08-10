using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface ICurrentUserContext
    {

        Guid BranchId { get; }

        string Role {  get; }
        Guid NameIdentifier { get; }

    }
}
