using MediatR.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } =  new Guid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is BaseEntity other && 
               other.GetType() == GetType() &&
               other.Id == Id;
    }

}
