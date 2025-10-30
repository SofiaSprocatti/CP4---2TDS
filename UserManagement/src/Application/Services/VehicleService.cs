using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Services;

public interface IVehicleService
{
    Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto);
    Task<VehicleResponseDto?> GetVehicleByIdAsync(Guid id);
    Task<IEnumerable<VehicleResponseDto>> GetAllVehiclesAsync();
    Task<IEnumerable<VehicleResponseDto>> GetAvailableVehiclesAsync();
    Task<IEnumerable<VehicleResponseDto>> GetVehiclesByStatusAsync(VehicleStatus status);
    Task<IEnumerable<VehicleResponseDto>> GetVehiclesByTypeAsync(VehicleType type);
    Task<VehicleResponseDto?> UpdateVehicleMileageAsync(Guid id, UpdateVehicleDto dto);
    Task<bool> MakeVehicleAvailableAsync(Guid id);
    Task<bool> SendVehicleToMaintenanceAsync(Guid id);
    Task<bool> RestrictVehicleAsync(Guid id, string reason);
    Task<bool> DeactivateVehicleAsync(Guid id);
    Task<bool> DeleteVehicleAsync(Guid id);
}

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleModelRepository _vehicleModelRepository;

    public VehicleService(IVehicleRepository vehicleRepository, IVehicleModelRepository vehicleModelRepository)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleModelRepository = vehicleModelRepository;
    }

    public async Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto)
    {
        if (await _vehicleRepository.ExistsByLicensePlateAsync(dto.LicensePlate))
            throw new InvalidOperationException("Vehicle with this license plate already exists");

        if (await _vehicleRepository.ExistsByChassisAsync(dto.Chassis))
            throw new InvalidOperationException("Vehicle with this chassis already exists");

        var vehicleModel = await _vehicleModelRepository.GetByIdAsync(dto.VehicleModelId);
        if (vehicleModel == null)
            throw new InvalidOperationException("Vehicle model not found");

        var vehicle = Vehicle.Create(dto.LicensePlate, dto.Chassis, dto.VehicleModelId, dto.Type);
        await _vehicleRepository.AddAsync(vehicle);

        return MapToResponseDto(vehicle);
    }

    public async Task<VehicleResponseDto?> GetVehicleByIdAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        return vehicle != null ? MapToResponseDto(vehicle) : null;
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetAllVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        var responseDtos = new List<VehicleResponseDto>();
        
        foreach (var vehicle in vehicles)
        {
            responseDtos.Add(MapToResponseDto(vehicle));
        }
        
        return responseDtos;
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetAvailableVehiclesAsync()
    {
        var vehicles = await _vehicleRepository.GetAvailableVehiclesAsync();
        var responseDtos = new List<VehicleResponseDto>();
        
        foreach (var vehicle in vehicles)
        {
            responseDtos.Add(MapToResponseDto(vehicle));
        }
        
        return responseDtos;
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetVehiclesByStatusAsync(VehicleStatus status)
    {
        var vehicles = await _vehicleRepository.GetByStatusAsync(status);
        var responseDtos = new List<VehicleResponseDto>();
        
        foreach (var vehicle in vehicles)
        {
            responseDtos.Add(MapToResponseDto(vehicle));
        }
        
        return responseDtos;
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetVehiclesByTypeAsync(VehicleType type)
    {
        var vehicles = await _vehicleRepository.GetByTypeAsync(type);
        var responseDtos = new List<VehicleResponseDto>();
        
        foreach (var vehicle in vehicles)
        {
            responseDtos.Add(MapToResponseDto(vehicle));
        }
        
        return responseDtos;
    }

    public async Task<VehicleResponseDto?> UpdateVehicleMileageAsync(Guid id, UpdateVehicleDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return null;

        vehicle.UpdateMileage(dto.Mileage);
        await _vehicleRepository.UpdateAsync(vehicle);
        return MapToResponseDto(vehicle);
    }

    public async Task<bool> MakeVehicleAvailableAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return false;

        vehicle.MakeAvailable();
        await _vehicleRepository.UpdateAsync(vehicle);
        return true;
    }

    public async Task<bool> SendVehicleToMaintenanceAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return false;

        vehicle.SendToMaintenance();
        await _vehicleRepository.UpdateAsync(vehicle);
        return true;
    }

    public async Task<bool> RestrictVehicleAsync(Guid id, string reason)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return false;

        vehicle.Restrict(reason);
        await _vehicleRepository.UpdateAsync(vehicle);
        return true;
    }

    public async Task<bool> DeactivateVehicleAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return false;

        vehicle.Deactivate();
        await _vehicleRepository.UpdateAsync(vehicle);
        return true;
    }

    public async Task<bool> DeleteVehicleAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return false;

        await _vehicleRepository.DeleteAsync(id);
        return true;
    }

    private VehicleResponseDto MapToResponseDto(Vehicle vehicle)
    {
        VehicleModelResponseDto? vehicleModelDto = null;
        
        if (vehicle.VehicleModel != null)
        {
            vehicleModelDto = new VehicleModelResponseDto(
                vehicle.VehicleModel.Id,
                vehicle.VehicleModel.Brand ?? string.Empty,
                vehicle.VehicleModel.Model ?? string.Empty,
                vehicle.VehicleModel.Year,
                vehicle.VehicleModel.FuelType ?? string.Empty,
                vehicle.VehicleModel.Engine ?? string.Empty,
                vehicle.VehicleModel.DailyRate,
                vehicle.VehicleModel.GetDisplayName(),
                vehicle.VehicleModel.IsActive,
                vehicle.VehicleModel.CreatedAt,
                vehicle.VehicleModel.UpdatedAt
            );
        }

        return new VehicleResponseDto(
            vehicle.Id,
            vehicle.LicensePlate?.Value ?? string.Empty,
            vehicle.Chassis?.Value ?? string.Empty,
            vehicle.VehicleModelId,
            vehicleModelDto,
            vehicle.Type,
            vehicle.Status,
            vehicle.Mileage,
            vehicle.RestrictionReason,
            vehicle.GetStatusDescription(),
            vehicle.IsAvailableForRent(),
            vehicle.CreatedAt,
            vehicle.UpdatedAt
        );
    }
}