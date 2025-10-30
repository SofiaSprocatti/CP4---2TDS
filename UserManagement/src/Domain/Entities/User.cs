using UserManagement.Domain.Common;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Domain.Entities;

public class User : BaseEntity
{
    public Name? Name { get; private set; }
    public Email? Email { get; private set; }
    public DocumentNumber? Document { get; private set; }
    public CNH? DriverLicense { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsEligibleToRent { get; private set; }

    private readonly List<Document> _documents = new();
    public IReadOnlyList<Document> Documents => _documents.AsReadOnly();

    private User() { }

    private User(Name name, Email email) : base()
    {
        Name = name;
        Email = email;
        IsActive = true;
        IsEligibleToRent = false;
    }

    public static User Create(string firstName, string lastName, string email)
    {
        var name = Name.Create(firstName, lastName);
        var emailValue = Email.Create(email);

        return new User(name, emailValue);
    }

    public void UpdateName(string firstName, string lastName)
    {
        Name = Name.Create(firstName, lastName);
        SetUpdatedAt();
    }

    public void UpdateEmail(string email)
    {
        Email = Email.Create(email);
        SetUpdatedAt();
    }

    public void AddDocument(string documentValue, string documentType)
    {
        Document = documentType.ToUpperInvariant() switch
        {
            "CPF" => DocumentNumber.CreateCPF(documentValue),
            "CNPJ" => DocumentNumber.CreateCNPJ(documentValue),
            _ => throw new ArgumentException("Invalid document type")
        };
        SetUpdatedAt();
    }

    public void AddDriverLicense(string cnhNumber, DateTime expiryDate, string category)
    {
        DriverLicense = CNH.Create(cnhNumber, expiryDate, category);
        UpdateEligibility();
        SetUpdatedAt();
    }

    public void UpdateDriverLicense(string cnhNumber, DateTime expiryDate, string category)
    {
        DriverLicense = CNH.Create(cnhNumber, expiryDate, category);
        UpdateEligibility();
        SetUpdatedAt();
    }

    private void UpdateEligibility()
    {
        IsEligibleToRent = IsActive && 
                          DriverLicense != null && 
                          DriverLicense.IsValid() && 
                          Document != null;
    }

    public void Activate()
    {
        IsActive = true;
        UpdateEligibility();
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        IsEligibleToRent = false;
        SetUpdatedAt();
    }

    public bool CanRentVehicleType(string vehicleType)
    {
        return IsEligibleToRent && 
               DriverLicense != null && 
               DriverLicense.CanDriveVehicleType(vehicleType);
    }

    public string GetDisplayName() => Name?.GetFullName() ?? "Unknown User";
}