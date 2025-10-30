using UserManagement.Domain.Common;
using UserManagement.Domain.Enums;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Domain.Entities;

public class Vehicle : BaseEntity
{
    public LicensePlate? LicensePlate { get; private set; }
    public Chassis? Chassis { get; private set; }
    public Guid VehicleModelId { get; private set; }
    public VehicleModel? VehicleModel { get; private set; }
    public VehicleType Type { get; private set; }
    public VehicleStatus Status { get; private set; }
    public int Mileage { get; private set; }
    public string? RestrictionReason { get; private set; }

    private Vehicle() { }

    private Vehicle(LicensePlate licensePlate, Chassis chassis, Guid vehicleModelId, VehicleType type) : base()
    {
        LicensePlate = licensePlate;
        Chassis = chassis;
        VehicleModelId = vehicleModelId;
        Type = type;
        Status = VehicleStatus.Available;
        Mileage = 0;
    }

    public static Vehicle Create(string licensePlate, string chassis, Guid vehicleModelId, VehicleType type)
    {
        var plateValue = LicensePlate.Create(licensePlate);
        var chassisValue = Chassis.Create(chassis);

        return new Vehicle(plateValue, chassisValue, vehicleModelId, type);
    }

    public void UpdateMileage(int newMileage)
    {
        if (newMileage < Mileage)
            throw new ArgumentException("New mileage cannot be less than current mileage");

        Mileage = newMileage;
        SetUpdatedAt();
    }

    public void MakeAvailable()
    {
        if (Status == VehicleStatus.Rented)
            throw new InvalidOperationException("Cannot make available a rented vehicle");

        Status = VehicleStatus.Available;
        RestrictionReason = null;
        SetUpdatedAt();
    }

    public void Rent()
    {
        if (Status != VehicleStatus.Available)
            throw new InvalidOperationException("Vehicle is not available for rent");

        Status = VehicleStatus.Rented;
        SetUpdatedAt();
    }

    public void ReturnFromRental()
    {
        if (Status != VehicleStatus.Rented)
            throw new InvalidOperationException("Vehicle is not currently rented");

        Status = VehicleStatus.Available;
        SetUpdatedAt();
    }

    public void SendToMaintenance()
    {
        if (Status == VehicleStatus.Rented)
            throw new InvalidOperationException("Cannot send a rented vehicle to maintenance");

        Status = VehicleStatus.Maintenance;
        SetUpdatedAt();
    }

    public void Restrict(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Restriction reason is required");

        if (Status == VehicleStatus.Rented)
            throw new InvalidOperationException("Cannot restrict a rented vehicle");

        Status = VehicleStatus.Restricted;
        RestrictionReason = reason;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        if (Status == VehicleStatus.Rented)
            throw new InvalidOperationException("Cannot deactivate a rented vehicle");

        Status = VehicleStatus.Deactivated;
        SetUpdatedAt();
    }

    public bool IsAvailableForRent() => Status == VehicleStatus.Available;

    public string GetStatusDescription()
    {
        return Status switch
        {
            VehicleStatus.Available => "Available",
            VehicleStatus.Rented => "Rented",
            VehicleStatus.Maintenance => "In Maintenance",
            VehicleStatus.Restricted => $"Restricted: {RestrictionReason}",
            VehicleStatus.Deactivated => "Deactivated",
            _ => "Unknown"
        };
    }
}