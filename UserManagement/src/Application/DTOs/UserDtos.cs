namespace UserManagement.Application.DTOs;

public record CreateUserDto(
    string FirstName,
    string LastName,
    string Email
);

public record UpdateUserDto(
    string FirstName,
    string LastName,
    string Email
);

public record AddDocumentToUserDto(
    string DocumentValue,
    string DocumentType 
);

public record AddDocumentDto(
    string DocumentValue,
    string DocumentType  
);

public record AddDriverLicenseDto(
    string CnhNumber,
    DateTime ExpiryDate,
    string Category
);

public record UserResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string FullName,
    string? Document,
    string? DocumentType,
    string? DriverLicenseNumber,
    string? DriverLicenseCategory,
    DateTime? DriverLicenseExpiry,
    bool IsDriverLicenseValid,
    bool IsActive,
    bool IsEligibleToRent,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);