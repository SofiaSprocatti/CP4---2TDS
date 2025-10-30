using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Services;
using UserManagement.Domain.Enums;

namespace UserManagement.Api.Controllers;

[Route("api/[controller]")]
public class VehicleModelsController : BaseController
{
    private readonly IVehicleModelService _vehicleModelService;

    public VehicleModelsController(IVehicleModelService vehicleModelService, ILogger<VehicleModelsController> logger)
        : base(logger)
    {
        _vehicleModelService = vehicleModelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicleModels([FromQuery] bool? activeOnly = null)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting vehicle models. ActiveOnly: {ActiveOnly}", activeOnly);
            
            return activeOnly == true
                ? await _vehicleModelService.GetActiveVehicleModelsAsync()
                : await _vehicleModelService.GetAllVehicleModelsAsync();
        }, "GetVehicleModels");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVehicleModel(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting vehicle model with ID: {VehicleModelId}", id);
            
            var vehicleModel = await _vehicleModelService.GetVehicleModelByIdAsync(id);
            if (vehicleModel == null)
            {
                _logger.LogWarning("Vehicle model not found. ID: {VehicleModelId}", id);
                throw new KeyNotFoundException($"Vehicle model with ID {id} not found");
            }

            _logger.LogInformation("Vehicle model found: {Brand} {Model}", vehicleModel.Brand, vehicleModel.Model);
            return vehicleModel;
        }, "GetVehicleModel");
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehicleModel([FromBody] CreateVehicleModelDto createVehicleModelDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Creating vehicle model: {Brand} {Model}", 
                createVehicleModelDto.Brand, createVehicleModelDto.Model);
            
            var vehicleModel = await _vehicleModelService.CreateVehicleModelAsync(createVehicleModelDto);
            
            _logger.LogInformation("Vehicle model created successfully: {VehicleModelId} - {Brand} {Model}", 
                vehicleModel.Id, vehicleModel.Brand, vehicleModel.Model);
            return vehicleModel;
        }, "CreateVehicleModel");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateVehicleModel(Guid id, [FromBody] UpdateVehicleModelDto updateVehicleModelDto)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Updating vehicle model: {VehicleModelId}", id);
            
            var vehicleModel = await _vehicleModelService.UpdateVehicleModelAsync(id, updateVehicleModelDto);
            if (vehicleModel == null)
            {
                _logger.LogWarning("Vehicle model not found for update. ID: {VehicleModelId}", id);
                throw new KeyNotFoundException($"Vehicle model with ID {id} not found");
            }

            _logger.LogInformation("Vehicle model updated successfully: {VehicleModelId}", id);
            return vehicleModel;
        }, "UpdateVehicleModel");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteVehicleModel(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Deleting vehicle model: {VehicleModelId}", id);
            
            var result = await _vehicleModelService.DeleteVehicleModelAsync(id);
            if (!result)
            {
                _logger.LogWarning("Vehicle model not found for deletion. ID: {VehicleModelId}", id);
                throw new KeyNotFoundException($"Vehicle model with ID {id} not found");
            }

            _logger.LogInformation("Vehicle model deleted successfully: {VehicleModelId}", id);
        }, "DeleteVehicleModel");
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> ActivateVehicleModel(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Activating vehicle model: {VehicleModelId}", id);
            
            var result = await _vehicleModelService.ActivateVehicleModelAsync(id);
            if (!result)
            {
                _logger.LogWarning("Vehicle model not found for activation. ID: {VehicleModelId}", id);
                throw new KeyNotFoundException($"Vehicle model with ID {id} not found");
            }

            _logger.LogInformation("Vehicle model activated successfully: {VehicleModelId}", id);
        }, "ActivateVehicleModel");
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateVehicleModel(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Deactivating vehicle model: {VehicleModelId}", id);
            
            var result = await _vehicleModelService.DeactivateVehicleModelAsync(id);
            if (!result)
            {
                _logger.LogWarning("Vehicle model not found for deactivation. ID: {VehicleModelId}", id);
                throw new KeyNotFoundException($"Vehicle model with ID {id} not found");
            }

            _logger.LogInformation("Vehicle model deactivated successfully: {VehicleModelId}", id);
        }, "DeactivateVehicleModel");
    }
}