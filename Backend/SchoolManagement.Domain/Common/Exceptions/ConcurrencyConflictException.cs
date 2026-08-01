namespace SchoolManagement.Domain.Common.Exceptions;

public class ConcurrencyConflictException : DomainException
{
    public ConcurrencyConflictException(string message) : base(message) { }
}
