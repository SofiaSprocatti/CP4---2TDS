namespace UserManagement.Domain.ValueObjects;

public class Name
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Name Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty");

        if (firstName.Length < 2 || firstName.Length > 50)
            throw new ArgumentException("First name must be between 2 and 50 characters");

        if (lastName.Length < 2 || lastName.Length > 50)
            throw new ArgumentException("Last name must be between 2 and 50 characters");

        return new Name(firstName.Trim(), lastName.Trim());
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public override string ToString() => GetFullName();

    public override bool Equals(object? obj)
    {
        return obj is Name other && 
               FirstName == other.FirstName && 
               LastName == other.LastName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName);
    }
}