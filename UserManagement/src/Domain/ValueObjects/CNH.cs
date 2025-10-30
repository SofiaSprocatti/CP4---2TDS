namespace UserManagement.Domain.ValueObjects;

public class CNH
{
    public string Number { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public string Category { get; private set; }

    private CNH(string number, DateTime expiryDate, string category)
    {
        Number = number;
        ExpiryDate = expiryDate;
        Category = category;
    }

    public static CNH Create(string number, DateTime expiryDate, string category)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("CNH number cannot be empty");

        var cleanNumber = number.Replace(" ", "").Replace(".", "").Replace("-", "");

        if (cleanNumber.Length != 11 || !cleanNumber.All(char.IsDigit))
            throw new ArgumentException("CNH number must have 11 digits");

        if (expiryDate <= DateTime.UtcNow)
            throw new ArgumentException("CNH is expired or invalid expiry date");

        if (!IsValidCategory(category))
            throw new ArgumentException("Invalid CNH category");

        return new CNH(cleanNumber, expiryDate, category.ToUpperInvariant());
    }

    private static bool IsValidCategory(string category)
    {
        var validCategories = new[] { "A", "B", "C", "D", "E", "AB", "AC", "AD", "AE" };
        return validCategories.Contains(category.ToUpperInvariant());
    }

    public bool IsValid() => ExpiryDate > DateTime.UtcNow;

    public bool CanDriveVehicleType(string vehicleType)
    {
        return vehicleType.ToUpperInvariant() switch
        {
            "MOTORCYCLE" => Category.Contains('A'),
            "CAR" => Category.Contains('B'),
            "TRUCK" => Category.Contains('C'),
            "BUS" => Category.Contains('D'),
            "ARTICULATED" => Category.Contains('E'),
            _ => false
        };
    }

    public override string ToString() => $"{Number} - {Category} - Valid until {ExpiryDate:dd/MM/yyyy}";

    public override bool Equals(object? obj)
    {
        return obj is CNH other && Number == other.Number;
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }
}