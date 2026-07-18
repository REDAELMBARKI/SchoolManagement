using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.ValueObjects
{
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
} 
