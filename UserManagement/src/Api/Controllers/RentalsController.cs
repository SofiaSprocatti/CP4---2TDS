using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Services;
using UserManagement.Domain.Enums;

namespace UserManagement.Api.Controllers;

[Route("api/[controller]")]
public class RentalsController : BaseController
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService, ILogger<RentalsController> logger)
        : base(logger)
    {
        _rentalService = rentalService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRentals([FromQuery] RentalStatus? status = null)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting rentals. Status: {Status}", status);
            
            if (status == RentalStatus.Active)
                return await _rentalService.GetActiveRentalsAsync();
            else if (status == RentalStatus.Overdue)
                return await _rentalService.GetOverdueRentalsAsync();
            else
                return await _rentalService.GetAllRentalsAsync();
        }, "GetRentals");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRental(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting rental with ID: {RentalId}", id);
            
            var rental = await _rentalService.GetRentalByIdAsync(id);
            if (rental == null)
            {
                _logger.LogWarning("Rental not found. ID: {RentalId}", id);
                throw new KeyNotFoundException($"Rental with ID {id} not found");
            }

            _logger.LogInformation("Rental found: {RentalId}", rental.Id);
            return rental;
        }, "GetRental");
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetRentalsByUser(Guid userId)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting rentals for user: {UserId}", userId);
            
            return await _rentalService.GetRentalsByUserIdAsync(userId);
        }, "GetRentalsByUser");
    }

    [HttpGet("vehicle/{vehicleId:guid}")]
    public async Task<IActionResult> GetRentalsByVehicle(Guid vehicleId)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting rentals for vehicle: {VehicleId}", vehicleId);
            
            return await _rentalService.GetRentalsByVehicleIdAsync(vehicleId);
        }, "GetRentalsByVehicle");
    }

    [HttpPost]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalDto createRentalDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Creating rental. User: {UserId}, Vehicle: {VehicleId}", 
                createRentalDto.UserId, createRentalDto.VehicleId);
            
            var rental = await _rentalService.CreateRentalAsync(createRentalDto);
            
            _logger.LogInformation("Rental created successfully: {RentalId}", rental.Id);
            return rental;
        }, "CreateRental");
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmRental(Guid id, [FromBody] ConfirmRentalDto confirmRentalDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Confirming rental: {RentalId}. Initial mileage: {Mileage}", 
                id, confirmRentalDto.InitialMileage);
            
            var result = await _rentalService.ConfirmRentalAsync(id);
            if (!result)
            {
                _logger.LogWarning("Rental not found for confirmation. ID: {RentalId}", id);
                throw new KeyNotFoundException($"Rental with ID {id} not found");
            }

            _logger.LogInformation("Rental confirmed successfully: {RentalId}", id);
        }, "ConfirmRental");
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> CompleteRental(Guid id, [FromBody] CompleteRentalDto completeRentalDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Completing rental: {RentalId}. Final mileage: {Mileage}", 
                id, completeRentalDto.FinalMileage);
            
            var result = await _rentalService.CompleteRentalAsync(id, completeRentalDto);
            if (result == null)
            {
                _logger.LogWarning("Rental not found for completion. ID: {RentalId}", id);
                throw new KeyNotFoundException($"Rental with ID {id} not found");
            }

            _logger.LogInformation("Rental completed successfully: {RentalId}", id);
            return result;
        }, "CompleteRental");
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelRental(Guid id, [FromBody] CancelRentalDto cancelRentalDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Cancelling rental: {RentalId}. Reason: {Reason}", 
                id, cancelRentalDto.CancellationReason);
            
            var result = await _rentalService.CancelRentalAsync(id, cancelRentalDto.CancellationReason);
            if (!result)
            {
                _logger.LogWarning("Rental not found for cancellation. ID: {RentalId}", id);
                throw new KeyNotFoundException($"Rental with ID {id} not found");
            }

            _logger.LogInformation("Rental cancelled successfully: {RentalId}", id);
        }, "CancelRental");
    }

    [HttpPatch("{id:guid}/extend")]
    public async Task<IActionResult> ExtendRental(Guid id, [FromBody] ExtendRentalDto extendRentalDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Extending rental: {RentalId}. New end date: {NewEndDate}", 
                id, extendRentalDto.NewEndDate);
            
            var result = await _rentalService.ExtendRentalAsync(id, extendRentalDto);
            if (result == null)
            {
                _logger.LogWarning("Rental not found for extension. ID: {RentalId}", id);
                throw new KeyNotFoundException($"Rental with ID {id} not found");
            }

            _logger.LogInformation("Rental extended successfully: {RentalId}", id);
            return result;
        }, "ExtendRental");
    }

    [HttpPatch("{id:guid}/mark-overdue")]
    public async Task<IActionResult> MarkAsOverdue(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Marking rental as overdue: {RentalId}", id);
            
            var result = await _rentalService.MarkAsOverdueAsync(id);
            if (!result)
            {
                _logger.LogWarning("Rental not found for overdue marking. ID: {RentalId}", id);
                throw new KeyNotFoundException($"Rental with ID {id} not found");
            }

            _logger.LogInformation("Rental marked as overdue successfully: {RentalId}", id);
        }, "MarkAsOverdue");
    }
}