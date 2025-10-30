using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Services;

public interface IVehicleModelService
{
    Task<VehicleModelResponseDto> CreateVehicleModelAsync(CreateVehicleModelDto dto);
    Task<VehicleModelResponseDto?> GetVehicleModelByIdAsync(Guid id);
    Task<IEnumerable<VehicleModelResponseDto>> GetAllVehicleModelsAsync();
    Task<IEnumerable<VehicleModelResponseDto>> GetActiveVehicleModelsAsync();
    Task<VehicleModelResponseDto?> UpdateVehicleModelAsync(Guid id, UpdateVehicleModelDto dto);
    Task<bool> DeleteVehicleModelAsync(Guid id);
    Task<bool> ActivateVehicleModelAsync(Guid id);
    Task<bool> DeactivateVehicleModelAsync(Guid id);
}

public class VehicleModelService : IVehicleModelService
{
    private readonly IVehicleModelRepository _vehicleModelRepository;

    public VehicleModelService(IVehicleModelRepository vehicleModelRepository)
    {
        _vehicleModelRepository = vehicleModelRepository;
    }

    public async Task<VehicleModelResponseDto> CreateVehicleModelAsync(CreateVehicleModelDto dto)
    {
        var vehicleModel = VehicleModel.Create(dto.Brand, dto.Model, dto.Year, dto.FuelType, dto.Engine, dto.DailyRate);
        await _vehicleModelRepository.AddAsync(vehicleModel);
        return MapToResponseDto(vehicleModel);
    }

    public async Task<VehicleModelResponseDto?> GetVehicleModelByIdAsync(Guid id)
    {
        var vehicleModel = await _vehicleModelRepository.GetByIdAsync(id);
        return vehicleModel != null ? MapToResponseDto(vehicleModel) : null;
    }

    public async Task<IEnumerable<VehicleModelResponseDto>> GetAllVehicleModelsAsync()
    {
        var vehicleModels = await _vehicleModelRepository.GetAllAsync();
        return vehicleModels.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<VehicleModelResponseDto>> GetActiveVehicleModelsAsync()
    {
        var vehicleModels = await _vehicleModelRepository.GetActiveModelsAsync();
        return vehicleModels.Select(MapToResponseDto);
    }

    public async Task<VehicleModelResponseDto?> UpdateVehicleModelAsync(Guid id, UpdateVehicleModelDto dto)
    {
        var vehicleModel = await _vehicleModelRepository.GetByIdAsync(id);
        if (vehicleModel == null) return null;

        vehicleModel.UpdateDetails(dto.Brand, dto.Model, dto.Year, dto.FuelType, dto.Engine, dto.DailyRate);
        await _vehicleModelRepository.UpdateAsync(vehicleModel);
        return MapToResponseDto(vehicleModel);
    }

    public async Task<bool> DeleteVehicleModelAsync(Guid id)
    {
        var vehicleModel = await _vehicleModelRepository.GetByIdAsync(id);
        if (vehicleModel == null) return false;

        await _vehicleModelRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ActivateVehicleModelAsync(Guid id)
    {
        var vehicleModel = await _vehicleModelRepository.GetByIdAsync(id);
        if (vehicleModel == null) return false;

        vehicleModel.Activate();
        await _vehicleModelRepository.UpdateAsync(vehicleModel);
        return true;
    }

    public async Task<bool> DeactivateVehicleModelAsync(Guid id)
    {
        var vehicleModel = await _vehicleModelRepository.GetByIdAsync(id);
        if (vehicleModel == null) return false;

        vehicleModel.Deactivate();
        await _vehicleModelRepository.UpdateAsync(vehicleModel);
        return true;
    }

    private static VehicleModelResponseDto MapToResponseDto(VehicleModel vehicleModel)
    {
        return new VehicleModelResponseDto(
            vehicleModel.Id,
            vehicleModel.Brand ?? string.Empty,
            vehicleModel.Model ?? string.Empty,
            vehicleModel.Year,
            vehicleModel.FuelType ?? string.Empty,
            vehicleModel.Engine ?? string.Empty,
            vehicleModel.DailyRate,
            vehicleModel.GetDisplayName(),
            vehicleModel.IsActive,
            vehicleModel.CreatedAt,
            vehicleModel.UpdatedAt
        );
    }
}