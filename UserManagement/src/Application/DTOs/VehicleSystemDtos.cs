using UserManagement.Domain.Enums;

namespace UserManagement.Application.DTOs;

public record CreateVehicleModelDto(
    string Brand,
    string Model,
    int Year,
    string FuelType,
    string Engine,
    decimal DailyRate
);

public record UpdateVehicleModelDto(
    string Brand,
    string Model,
    int Year,
    string FuelType,
    string Engine,
    decimal DailyRate
);

public record VehicleModelResponseDto(
    Guid Id,
    string Brand,
    string Model,
    int Year,
    string FuelType,
    string Engine,
    decimal DailyRate,
    string DisplayName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateVehicleDto(
    string LicensePlate,
    string Chassis,
    Guid VehicleModelId,
    VehicleType Type
);

public record UpdateVehicleDto(
    int Mileage
);

public record UpdateMileageDto(
    int Mileage
);

public record MaintenanceDto(
    string Reason
);

public record RestrictionDto(
    string Reason
);

public record VehicleResponseDto(
    Guid Id,
    string LicensePlate,
    string Chassis,
    Guid VehicleModelId,
    VehicleModelResponseDto? VehicleModel,
    VehicleType Type,
    VehicleStatus Status,
    int Mileage,
    string? RestrictionReason,
    string StatusDescription,
    bool IsAvailableForRent,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateDocumentDto(
    DocumentType Type,
    string Number,
    string FilePath,
    DateTime? ExpiryDate = null,
    Guid? UserId = null,
    Guid? VehicleId = null
);

public record DocumentResponseDto(
    Guid Id,
    DocumentType Type,
    string TypeDescription,
    string Number,
    string FilePath,
    DateTime? ExpiryDate,
    bool IsExpired,
    Guid? UserId,
    Guid? VehicleId,
    Guid? RentalId,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateRentalDto(
    Guid UserId,
    Guid VehicleId,
    DateTime StartDate,
    DateTime EndDate
);

public record ConfirmRentalDto(
    int InitialMileage,
    string? Notes = null
);

public record CompleteRentalDto(
    int FinalMileage,
    string? Notes = null
);

public record CancelRentalDto(
    string CancellationReason
);

public record ExtendRentalDto(
    DateTime NewEndDate
);

public record RentalResponseDto(
    Guid Id,
    Guid UserId,
    UserResponseDto? User,
    Guid VehicleId,
    VehicleResponseDto? Vehicle,
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ActualReturnDate,
    decimal DailyRate,
    decimal TotalAmount,
    decimal? FinalAmount,
    RentalStatus Status,
    string StatusDescription,
    string? Notes,
    int? FinalMileage,
    int Duration,
    int? ActualDuration,
    bool IsOverdue,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);