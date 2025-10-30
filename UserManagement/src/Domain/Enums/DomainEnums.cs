namespace UserManagement.Domain.Enums;

public enum VehicleStatus
{
    Available = 1,
    Rented = 2,
    Maintenance = 3,
    Restricted = 4,
    Deactivated = 5
}

public enum VehicleType
{
    Car = 1,
    Motorcycle = 2,
    Truck = 3,
    Bus = 4
}

public enum RentalStatus
{
    Pending = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4,
    Overdue = 5
}

public enum DocumentType
{
    CNH = 1,
    RentalContract = 2,
    VehicleDocument = 3,
    Insurance = 4
}