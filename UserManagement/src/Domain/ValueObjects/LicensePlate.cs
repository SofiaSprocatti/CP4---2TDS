namespace UserManagement.Domain.ValueObjects;

public class LicensePlate
{
    public string Value { get; private set; }

    private LicensePlate(string value)
    {
        Value = value;
    }

    public static LicensePlate Create(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new ArgumentException("License plate cannot be empty");

        var cleanPlate = licensePlate.Replace("-", "").Replace(" ", "").ToUpperInvariant();

        if (!IsValidBrazilianPlate(cleanPlate))
            throw new ArgumentException("Invalid Brazilian license plate format");

        return new LicensePlate(FormatPlate(cleanPlate));
    }

    private static bool IsValidBrazilianPlate(string plate)
    {
        if (plate.Length != 7) return false;

        if (char.IsLetter(plate[0]) && char.IsLetter(plate[1]) && char.IsLetter(plate[2]) &&
            char.IsDigit(plate[3]) && char.IsDigit(plate[4]) && char.IsDigit(plate[5]) && char.IsDigit(plate[6]))
            return true;

        if (char.IsLetter(plate[0]) && char.IsLetter(plate[1]) && char.IsLetter(plate[2]) &&
            char.IsDigit(plate[3]) && char.IsLetter(plate[4]) && char.IsDigit(plate[5]) && char.IsDigit(plate[6]))
            return true;

        return false;
    }

    private static string FormatPlate(string plate)
    {
        return $"{plate.Substring(0, 3)}-{plate.Substring(3)}";
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        return obj is LicensePlate other && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}