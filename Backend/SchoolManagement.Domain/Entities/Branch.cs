using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using System;

namespace SchoolManagement.Domain.Entities;

public class Branch : AggregateRoot
{
    public string Name { get; set; }  = string.Empty ;
    public string Slug {get;set;} = string.Empty ;
    public string City { get; set; }  = string.Empty ;
    public string Address { get; set; }  = string.Empty ;
    public string? Phone { get; set; }

    public virtual ICollection<Enrollment> Enrollments {get;set;} = new List<Enrollment>();

}