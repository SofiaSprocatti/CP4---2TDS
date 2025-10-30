using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Services;
using UserManagement.Domain.Enums;

namespace UserManagement.Api.Controllers;

[Route("api/[controller]")]
public class VehiclesController : BaseController
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
        : base(logger)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicles(
        [FromQuery] VehicleStatus? status = null,
        [FromQuery] VehicleType? type = null,
        [FromQuery] bool? availableOnly = null)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting vehicles. Status: {Status}, Type: {Type}, AvailableOnly: {AvailableOnly}", 
                status, type, availableOnly);
            
            if (availableOnly == true)
                return await _vehicleService.GetAvailableVehiclesAsync();
            else if (status.HasValue)
                return await _vehicleService.GetVehiclesByStatusAsync(status.Value);
            else if (type.HasValue)
                return await _vehicleService.GetVehiclesByTypeAsync(type.Value);
            else
                return await _vehicleService.GetAllVehiclesAsync();
        }, "GetVehicles");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVehicle(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting vehicle with ID: {VehicleId}", id);
            
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle not found. ID: {VehicleId}", id);
                throw new KeyNotFoundException($"Vehicle with ID {id} not found");
            }

            _logger.LogInformation("Vehicle found: {LicensePlate}", vehicle.LicensePlate);
            return vehicle;
        }, "GetVehicle");
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto createVehicleDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Creating vehicle: {LicensePlate}", createVehicleDto.LicensePlate);
            
            var vehicle = await _vehicleService.CreateVehicleAsync(createVehicleDto);
            
            _logger.LogInformation("Vehicle created successfully: {VehicleId} - {LicensePlate}", 
                vehicle.Id, vehicle.LicensePlate);
            return vehicle;
        }, "CreateVehicle");
    }

    [HttpPut("{id:guid}/mileage")]
    public async Task<IActionResult> UpdateMileage(Guid id, [FromBody] UpdateMileageDto updateMileageDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Updating vehicle mileage: {VehicleId} to {Mileage}", 
                id, updateMileageDto.Mileage);
            
            var updateDto = new UpdateVehicleDto(updateMileageDto.Mileage);
            var result = await _vehicleService.UpdateVehicleMileageAsync(id, updateDto);
            if (result == null)
            {
                _logger.LogWarning("Vehicle not found for mileage update. ID: {VehicleId}", id);
                throw new KeyNotFoundException($"Vehicle with ID {id} not found");
            }

            _logger.LogInformation("Vehicle mileage updated successfully: {VehicleId}", id);
            return result;
        }, "UpdateMileage");
    }

    [HttpPatch("{id:guid}/make-available")]
    public async Task<IActionResult> MakeAvailable(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Making vehicle available: {VehicleId}", id);
            
            var result = await _vehicleService.MakeVehicleAvailableAsync(id);
            if (!result)
            {
                _logger.LogWarning("Vehicle not found for status change. ID: {VehicleId}", id);
                throw new KeyNotFoundException($"Vehicle with ID {id} not found");
            }

            _logger.LogInformation("Vehicle made available successfully: {VehicleId}", id);
        }, "MakeAvailable");
    }

    [HttpPatch("{id:guid}/maintenance")]
    public async Task<IActionResult> SendToMaintenance(Guid id, [FromBody] MaintenanceDto maintenanceDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Sending vehicle to maintenance: {VehicleId}. Reason: {Reason}", 
                id, maintenanceDto.Reason);
            
            var result = await _vehicleService.SendVehicleToMaintenanceAsync(id);
            if (!result)
            {
                _logger.LogWarning("Vehicle not found for maintenance. ID: {VehicleId}", id);
                throw new KeyNotFoundException($"Vehicle with ID {id} not found");
            }

            _logger.LogInformation("Vehicle sent to maintenance successfully: {VehicleId}", id);
        }, "SendToMaintenance");
    }

    [HttpPatch("{id:guid}/restrict")]
    public async Task<IActionResult> RestrictVehicle(Guid id, [FromBody] RestrictionDto restrictionDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Restricting vehicle: {VehicleId}. Reason: {Reason}", 
                id, restrictionDto.Reason);
            
            var result = await _vehicleService.RestrictVehicleAsync(id, restrictionDto.Reason);
            if (!result)
            {
                _logger.LogWarning("Vehicle not found for restriction. ID: {VehicleId}", id);
                throw new KeyNotFoundException($"Vehicle with ID {id} not found");
            }

            _logger.LogInformation("Vehicle restricted successfully: {VehicleId}", id);
        }, "RestrictVehicle");
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateVehicle(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Deactivating vehicle: {VehicleId}", id);
            
            var result = await _vehicleService.DeactivateVehicleAsync(id);
            if (!result)
            {
                _logger.LogWarning("Vehicle not found for deactivation. ID: {VehicleId}", id);
                throw new KeyNotFoundException($"Vehicle with ID {id} not found");
            }

            _logger.LogInformation("Vehicle deactivated successfully: {VehicleId}", id);
        }, "DeactivateVehicle");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Deleting vehicle: {VehicleId}", id);
            
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result)
            {
                _logger.LogWarning("Vehicle not found for deletion. ID: {VehicleId}", id);
                throw new KeyNotFoundException($"Vehicle with ID {id} not found");
            }

            _logger.LogInformation("Vehicle deleted successfully: {VehicleId}", id);
        }, "DeleteVehicle");
    }
}