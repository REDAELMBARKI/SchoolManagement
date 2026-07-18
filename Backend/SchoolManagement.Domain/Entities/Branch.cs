using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Exceptions;

namespace SchoolManagement.Domain.Entities;

public class Branch : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string? Phone { get; private set; }

    public virtual ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

    private Branch() { }

    public static Branch Create(string name, string slug, string city, string address, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Branch name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Branch slug cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("Branch city cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new DomainException("Branch address cannot be empty.");
        }

        return new Branch
        {
            Name = name,
            Slug = slug,
            City = city,
            Address = address,
            Phone = phone
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Branch name cannot be empty.");
        }
        Name = name;
    }

    public void UpdateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Branch slug cannot be empty.");
        }
        Slug = slug;
    }

    public void UpdateCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("Branch city cannot be empty.");
        }
        City = city;
    }

    public void UpdateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new DomainException("Branch address cannot be empty.");
        }
        Address = address;
    }

    public void UpdatePhone(string? phone)
    {
        Phone = phone;
    }
}
