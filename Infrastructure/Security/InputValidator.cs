using System.Text.RegularExpressions;

namespace CampsiteBooking.Infrastructure.Security;

/// <summary>
/// Input validation helper to prevent XSS attacks.
/// Validates and sanitizes user input.
/// </summary>
public static class InputValidator
{
    // Regex to detect potential XSS patterns
    private static readonly Regex XssPattern = new(
        @"<script|</script|javascript:|onerror=|onload=|<iframe|eval\(|expression\(",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Regex to detect SQL injection patterns (defense in depth)
    private static readonly Regex SqlInjectionPattern = new(
        @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE)\b)|(')|(--)|(;)|(/\*)|(\*/)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Validate string input for XSS patterns.
    /// Throws exception if malicious content detected.
    /// </summary>
    public static void ValidateForXss(string input, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        if (XssPattern.IsMatch(input))
        {
            throw new SecurityException($"Potential XSS attack detected in {fieldName}");
        }
    }

    /// <summary>
    /// Validate string input for SQL injection patterns (defense in depth).
    /// Note: EF Core already protects against SQL injection, this is extra layer.
    /// </summary>
    public static void ValidateForSqlInjection(string input, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        if (SqlInjectionPattern.IsMatch(input))
        {
            throw new SecurityException($"Potential SQL injection detected in {fieldName}");
        }
    }

    /// <summary>
    /// Sanitize string by removing HTML tags.
    /// Use for user-generated content that will be displayed.
    /// </summary>
    public static string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove HTML tags
        var sanitized = Regex.Replace(input, @"<[^>]*>", string.Empty);
        
        // Decode HTML entities to prevent double-encoding
        sanitized = System.Net.WebUtility.HtmlDecode(sanitized);
        
        return sanitized;
    }

    /// <summary>
    /// Validate that string length is within acceptable range.
    /// </summary>
    public static void ValidateLength(string input, string fieldName, int maxLength, int minLength = 0)
    {
        if (input == null)
            return;

        if (input.Length < minLength)
        {
            throw new ArgumentException($"{fieldName} must be at least {minLength} characters");
        }

        if (input.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} cannot exceed {maxLength} characters");
        }
    }

    /// <summary>
    /// Validate email format (basic validation).
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Custom exception for security violations.
/// </summary>
public class SecurityException : Exception
{
    public SecurityException(string message) : base(message)
    {
    }

    public SecurityException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

