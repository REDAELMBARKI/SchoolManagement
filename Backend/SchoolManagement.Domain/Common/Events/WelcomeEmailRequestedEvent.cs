using MediatR;

namespace SchoolManagement.Domain.Common.Events;

/// <summary>
/// Event raised when a new user successfully registers
/// </summary>
public class WelcomeEmailRequestedEvent : INotification
{
    public string Email { get; }
    public string UserName { get; }
    public DateTime RegisteredAt { get; }

    public WelcomeEmailRequestedEvent(string email, string userName)
    {
        Email = email;
        UserName = userName;
        RegisteredAt = DateTime.UtcNow;
    }
}
