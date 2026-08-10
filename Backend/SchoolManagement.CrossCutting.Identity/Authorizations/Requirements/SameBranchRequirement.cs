using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.CrossCutting.Identity.Authorizations.Requirements
{
    public class SameBranchRequirement : IAuthorizationRequirement
    {
    }
}
