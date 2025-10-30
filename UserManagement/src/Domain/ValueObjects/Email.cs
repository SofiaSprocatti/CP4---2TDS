namespace UserManagement.Domain.ValueObjects;

public class Email
{
    public string Value { get; private set; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format");

        return new Email(email.ToLowerInvariant());
    }

    private static bool IsValidEmail(string email)
    {
        return email.Contains('@') && email.Contains('.') && email.Length > 5;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is Email other && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}