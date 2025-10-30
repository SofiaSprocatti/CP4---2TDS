using UserManagement.Domain.Common;

namespace UserManagement.Domain.Entities;

public class VehicleModel : BaseEntity
{
    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public int Year { get; private set; }
    public string? FuelType { get; private set; }
    public string? Engine { get; private set; }
    public decimal DailyRate { get; private set; }
    public bool IsActive { get; private set; }

    private VehicleModel() { }

    private VehicleModel(string brand, string model, int year, string fuelType, string engine, decimal dailyRate) : base()
    {
        Brand = brand;
        Model = model;
        Year = year;
        FuelType = fuelType;
        Engine = engine;
        DailyRate = dailyRate;
        IsActive = true;
    }

    public static VehicleModel Create(string brand, string model, int year, string fuelType, string engine, decimal dailyRate)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Brand cannot be empty");

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty");

        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid year");

        if (string.IsNullOrWhiteSpace(fuelType))
            throw new ArgumentException("Fuel type cannot be empty");

        if (string.IsNullOrWhiteSpace(engine))
            throw new ArgumentException("Engine cannot be empty");

        if (dailyRate <= 0)
            throw new ArgumentException("Daily rate must be greater than zero");

        return new VehicleModel(brand.Trim(), model.Trim(), year, fuelType.Trim(), engine.Trim(), dailyRate);
    }

    public void UpdateDetails(string brand, string model, int year, string fuelType, string engine, decimal dailyRate)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Brand cannot be empty");

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty");

        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid year");

        if (dailyRate <= 0)
            throw new ArgumentException("Daily rate must be greater than zero");

        Brand = brand.Trim();
        Model = model.Trim();
        Year = year;
        FuelType = fuelType.Trim();
        Engine = engine.Trim();
        DailyRate = dailyRate;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public string GetDisplayName() => $"{Brand} {Model} {Year}";
}