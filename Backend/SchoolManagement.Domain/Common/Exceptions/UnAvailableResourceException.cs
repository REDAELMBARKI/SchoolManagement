namespace SchoolManagement.Domain.Common.Exceptions;

public class UnAvailableResourceException : DomainException
{
    public UnAvailableResourceException(string message) : base(message)
    {
    }
}
