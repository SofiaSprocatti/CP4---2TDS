using UserManagement.Domain.Common;
using UserManagement.Domain.Enums;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Domain.Entities;

public class Document : BaseEntity
{
    public DocumentType Type { get; private set; }
    public string? Number { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? FilePath { get; private set; }
    public Guid? UserId { get; private set; }
    public User? User { get; private set; }
    public Guid? VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }
    public Guid? RentalId { get; private set; }
    public bool IsActive { get; private set; }

    private Document() { }

    private Document(DocumentType type, string number, string filePath, DateTime? expiryDate = null) : base()
    {
        Type = type;
        Number = number;
        FilePath = filePath;
        ExpiryDate = expiryDate;
        IsActive = true;
    }

    public static Document Create(DocumentType type, string number, string filePath, DateTime? expiryDate = null)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Document number cannot be empty");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty");

        if (expiryDate.HasValue && expiryDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Document is expired");

        return new Document(type, number.Trim(), filePath.Trim(), expiryDate);
    }

    public void AssignToUser(Guid userId)
    {
        UserId = userId;
        SetUpdatedAt();
    }

    public void AssignToVehicle(Guid vehicleId)
    {
        VehicleId = vehicleId;
        SetUpdatedAt();
    }

    public void AssignToRental(Guid rentalId)
    {
        RentalId = rentalId;
        SetUpdatedAt();
    }

    public void UpdateFilePath(string newFilePath)
    {
        if (string.IsNullOrWhiteSpace(newFilePath))
            throw new ArgumentException("File path cannot be empty");

        FilePath = newFilePath.Trim();
        SetUpdatedAt();
    }

    public void UpdateExpiryDate(DateTime? newExpiryDate)
    {
        if (newExpiryDate.HasValue && newExpiryDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Document is expired");

        ExpiryDate = newExpiryDate;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public bool IsExpired() => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;

    public string GetTypeDescription()
    {
        return Type switch
        {
            DocumentType.CNH => "Driver's License",
            DocumentType.RentalContract => "Rental Contract",
            DocumentType.VehicleDocument => "Vehicle Document",
            DocumentType.Insurance => "Insurance",
            _ => "Unknown"
        };
    }
}