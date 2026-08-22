namespace SchoolManagement.Domain.Common.Exceptions;

public class MyValidationException : Exception
{
    public MyValidationException(string message) : base(message) { }
    
    public MyValidationException(string message, Exception innerException) : base(message, innerException) { }
}
