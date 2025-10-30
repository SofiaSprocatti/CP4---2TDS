namespace UserManagement.Domain.ValueObjects;

public class Chassis
{
    public string Value { get; private set; }

    private Chassis(string value)
    {
        Value = value;
    }

    public static Chassis Create(string chassis)
    {
        if (string.IsNullOrWhiteSpace(chassis))
            throw new ArgumentException("Chassis cannot be empty");

        var cleanChassis = chassis.Replace(" ", "").ToUpperInvariant();

        if (!IsValidChassis(cleanChassis))
            throw new ArgumentException("Invalid chassis format - must be 17 characters");

        return new Chassis(cleanChassis);
    }

    private static bool IsValidChassis(string chassis)
    {
        if (chassis.Length != 17) return false;
        return !chassis.Contains('I') && !chassis.Contains('O') && !chassis.Contains('Q');
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is Chassis other && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}