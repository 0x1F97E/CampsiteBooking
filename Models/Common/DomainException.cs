namespace CampsiteBooking.Models.Common;

/// <summary>
/// Exception thrown when a domain rule is violated.
/// This represents a business rule violation, not a technical error.
/// </summary>
public class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

