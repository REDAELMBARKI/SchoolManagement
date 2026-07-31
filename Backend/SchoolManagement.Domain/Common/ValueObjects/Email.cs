using System;
using System.Collections.Generic;
using System.Text;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Common.Exceptions;

namespace SchoolManagement.Domain.Common.ValueObjects;

public class Email : ValueObject
{

    public string Value { get; }

    public Email(string value)
    {
        if (!value.Contains("@"))
            throw new DomainException("Invalid email");
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
} 
