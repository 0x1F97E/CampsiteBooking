using System.Net.Mail;

namespace CampsiteBooking.Models.ValueObjects;

/// <summary>
/// Value Object representing an email address.
/// Ensures email is always valid when created.
/// </summary>
public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        if (!IsValidEmail(value))
            throw new ArgumentException($"'{value}' is not a valid email address", nameof(value));

        Value = value.ToLowerInvariant(); // Normalize to lowercase
    }

    public static Email Create(string value)
    {
        return new Email(value);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    // Implicit conversion to string for convenience
    public static implicit operator string(Email email) => email.Value;
}

