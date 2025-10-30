using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Services;

namespace UserManagement.Api.Controllers;

[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService, ILogger<UsersController> logger) 
        : base(logger)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] bool? activeOnly = null)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting users. ActiveOnly: {ActiveOnly}", activeOnly);
            
            return activeOnly == true
                ? await _userService.GetActiveUsersAsync()
                : await _userService.GetAllUsersAsync();
        }, "GetUsers");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting user with ID: {UserId}", id);
            
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found. ID: {UserId}", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            _logger.LogInformation("User found: {Email}", user.Email);
            return user;
        }, "GetUser");
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Creating user: {Email}", createUserDto.Email);
            
            var user = await _userService.CreateUserAsync(createUserDto);
            
            _logger.LogInformation("User created successfully: {UserId} - {Email}", user.Id, user.Email);
            return user;
        }, "CreateUser");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Updating user: {UserId}", id);
            
            var user = await _userService.UpdateUserAsync(id, updateUserDto);
            if (user == null)
            {
                _logger.LogWarning("User not found for update. ID: {UserId}", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            _logger.LogInformation("User updated successfully: {UserId}", id);
            return user;
        }, "UpdateUser");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Deleting user: {UserId}", id);
            
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                _logger.LogWarning("User not found for deletion. ID: {UserId}", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            _logger.LogInformation("User deleted successfully: {UserId}", id);
        }, "DeleteUser");
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Activating user: {UserId}", id);
            
            var result = await _userService.ActivateUserAsync(id);
            if (!result)
            {
                _logger.LogWarning("User not found for activation. ID: {UserId}", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            _logger.LogInformation("User activated successfully: {UserId}", id);
        }, "ActivateUser");
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Deactivating user: {UserId}", id);
            
            var result = await _userService.DeactivateUserAsync(id);
            if (!result)
            {
                _logger.LogWarning("User not found for deactivation. ID: {UserId}", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            _logger.LogInformation("User deactivated successfully: {UserId}", id);
        }, "DeactivateUser");
    }

    [HttpPost("{id:guid}/document")]
    public async Task<IActionResult> AddDocument(Guid id, [FromBody] AddDocumentDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Adding document to user: {UserId}. Type: {DocumentType}", id, dto.DocumentType);
            
            var addDocumentDto = new AddDocumentToUserDto(dto.DocumentValue, dto.DocumentType);
            var result = await _userService.AddDocumentToUserAsync(id, addDocumentDto);
            if (!result)
            {
                _logger.LogWarning("User not found for document addition. ID: {UserId}", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            _logger.LogInformation("Document added successfully to user: {UserId}", id);
        }, "AddDocument");
    }

    [HttpPost("{id:guid}/driver-license")]
    public async Task<IActionResult> AddDriverLicense(Guid id, [FromBody] AddDriverLicenseDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Adding driver license to user: {UserId}. CNH: {CnhNumber}", id, dto.CnhNumber);
            
            var result = await _userService.AddDriverLicenseAsync(id, dto);
            if (!result)
            {
                _logger.LogWarning("User not found for driver license addition. ID: {UserId}", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            _logger.LogInformation("Driver license added successfully to user: {UserId}", id);
        }, "AddDriverLicense");
    }

    [HttpGet("{id:guid}/can-rent/{vehicleType}")]
    public async Task<IActionResult> CanUserRentVehicleType(Guid id, string vehicleType)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Checking if user can rent vehicle type. UserId: {UserId}, VehicleType: {VehicleType}", 
                id, vehicleType);
            
            var canRent = await _userService.CanUserRentVehicleTypeAsync(id, vehicleType);
            
            _logger.LogInformation("User rental eligibility check result: {CanRent} for user: {UserId}", canRent, id);
            return canRent;
        }, "CanUserRentVehicleType");
    }
}