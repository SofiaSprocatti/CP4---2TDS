using Microsoft.Extensions.Logging;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Services;

public interface IUserService
{
    Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<IEnumerable<UserResponseDto>> GetActiveUsersAsync();
    Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> ActivateUserAsync(Guid id);
    Task<bool> DeactivateUserAsync(Guid id);
    Task<bool> AddDocumentToUserAsync(Guid id, AddDocumentToUserDto dto);
    Task<bool> AddDriverLicenseAsync(Guid id, AddDriverLicenseDto dto);
    Task<bool> CanUserRentVehicleTypeAsync(Guid userId, string vehicleType);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        _logger.LogInformation("Creating user with email: {Email}", createUserDto.Email);
        
        if (await _userRepository.ExistsByEmailAsync(createUserDto.Email))
        {
            _logger.LogWarning("User creation failed - email already exists: {Email}", createUserDto.Email);
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = User.Create(createUserDto.FirstName, createUserDto.LastName, createUserDto.Email);
        await _userRepository.AddAsync(user);

        _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
        return MapToResponseDto(user);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToResponseDto(user) : null;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<UserResponseDto>> GetActiveUsersAsync()
    {
        var users = await _userRepository.GetActiveUsersAsync();
        return users.Select(MapToResponseDto);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        var existingUser = await _userRepository.GetByEmailAsync(updateUserDto.Email);
        if (existingUser != null && existingUser.Id != id)
            throw new InvalidOperationException("Another user with this email already exists");

        user.UpdateName(updateUserDto.FirstName, updateUserDto.LastName);
        user.UpdateEmail(updateUserDto.Email);

        await _userRepository.UpdateAsync(user);
        return MapToResponseDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ActivateUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        user.Activate();
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        user.Deactivate();
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> AddDocumentToUserAsync(Guid id, AddDocumentToUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        user.AddDocument(dto.DocumentValue, dto.DocumentType);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> AddDriverLicenseAsync(Guid id, AddDriverLicenseDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        user.AddDriverLicense(dto.CnhNumber, dto.ExpiryDate, dto.Category);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> CanUserRentVehicleTypeAsync(Guid userId, string vehicleType)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        return user.CanRentVehicleType(vehicleType);
    }

    private static UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto(
            user.Id,
            user.Name?.FirstName ?? string.Empty,
            user.Name?.LastName ?? string.Empty,
            user.Email?.Value ?? string.Empty,
            user.GetDisplayName(),
            user.Document?.Value,
            user.Document?.Type,
            user.DriverLicense?.Number,
            user.DriverLicense?.Category,
            user.DriverLicense?.ExpiryDate,
            user.DriverLicense?.IsValid() ?? false,
            user.IsActive,
            user.IsEligibleToRent,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}