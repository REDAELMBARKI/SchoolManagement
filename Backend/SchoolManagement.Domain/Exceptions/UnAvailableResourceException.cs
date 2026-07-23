namespace SchoolManagement.Domain.Exceptions;

public class UnAvailableResourceException : DomainException
{
    public UnAvailableResourceException(string message) : base(message)
    {
    }
}
