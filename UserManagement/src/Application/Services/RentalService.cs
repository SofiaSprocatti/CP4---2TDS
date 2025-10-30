using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;

namespace UserManagement.Application.Services;

public interface IRentalService
{
    Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto dto);
    Task<RentalResponseDto?> GetRentalByIdAsync(Guid id);
    Task<IEnumerable<RentalResponseDto>> GetAllRentalsAsync();
    Task<IEnumerable<RentalResponseDto>> GetRentalsByUserIdAsync(Guid userId);
    Task<IEnumerable<RentalResponseDto>> GetRentalsByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<RentalResponseDto>> GetActiveRentalsAsync();
    Task<IEnumerable<RentalResponseDto>> GetOverdueRentalsAsync();
    Task<bool> ConfirmRentalAsync(Guid id);
    Task<RentalResponseDto?> CompleteRentalAsync(Guid id, CompleteRentalDto dto);
    Task<bool> CancelRentalAsync(Guid id, string reason);
    Task<RentalResponseDto?> ExtendRentalAsync(Guid id, ExtendRentalDto dto);
    Task<bool> MarkAsOverdueAsync(Guid id);
}

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleModelRepository _vehicleModelRepository;

    public RentalService(
        IRentalRepository rentalRepository,
        IUserRepository userRepository,
        IVehicleRepository vehicleRepository,
        IVehicleModelRepository vehicleModelRepository)
    {
        _rentalRepository = rentalRepository;
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _vehicleModelRepository = vehicleModelRepository;
    }

    public async Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (!user.IsEligibleToRent)
            throw new InvalidOperationException("User is not eligible to rent");

        var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId);
        if (vehicle == null)
            throw new InvalidOperationException("Vehicle not found");

        if (!vehicle.IsAvailableForRent())
            throw new InvalidOperationException("Vehicle is not available for rent");

        var existingRental = await _rentalRepository.GetActiveRentalByUserIdAsync(dto.UserId);
        if (existingRental != null)
            throw new InvalidOperationException("User already has an active rental");

        var vehicleRental = await _rentalRepository.GetActiveRentalByVehicleIdAsync(dto.VehicleId);
        if (vehicleRental != null)
            throw new InvalidOperationException("Vehicle is already rented");

        var vehicleModel = await _vehicleModelRepository.GetByIdAsync(vehicle.VehicleModelId);
        if (vehicleModel == null)
            throw new InvalidOperationException("Vehicle model not found");

        if (!user.CanRentVehicleType(vehicle.Type.ToString()))
            throw new InvalidOperationException("User's driver license doesn't allow this vehicle type");

        var rental = Rental.Create(dto.UserId, dto.VehicleId, dto.StartDate, dto.EndDate, vehicleModel.DailyRate);
        await _rentalRepository.AddAsync(rental);

        return MapToResponseDto(rental);
    }

    public async Task<RentalResponseDto?> GetRentalByIdAsync(Guid id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        return rental != null ? MapToResponseDto(rental) : null;
    }

    public async Task<IEnumerable<RentalResponseDto>> GetAllRentalsAsync()
    {
        var rentals = await _rentalRepository.GetAllAsync();
        var responseDtos = new List<RentalResponseDto>();
        
        foreach (var rental in rentals)
        {
            responseDtos.Add(MapToResponseDto(rental));
        }
        
        return responseDtos;
    }

    public async Task<IEnumerable<RentalResponseDto>> GetRentalsByUserIdAsync(Guid userId)
    {
        var rentals = await _rentalRepository.GetByUserIdAsync(userId);
        var responseDtos = new List<RentalResponseDto>();
        
        foreach (var rental in rentals)
        {
            responseDtos.Add(MapToResponseDto(rental));
        }
        
        return responseDtos;
    }

    public async Task<IEnumerable<RentalResponseDto>> GetRentalsByVehicleIdAsync(Guid vehicleId)
    {
        var rentals = await _rentalRepository.GetByVehicleIdAsync(vehicleId);
        var responseDtos = new List<RentalResponseDto>();
        
        foreach (var rental in rentals)
        {
            responseDtos.Add(MapToResponseDto(rental));
        }
        
        return responseDtos;
    }

    public async Task<IEnumerable<RentalResponseDto>> GetActiveRentalsAsync()
    {
        var rentals = await _rentalRepository.GetActiveRentalsAsync();
        var responseDtos = new List<RentalResponseDto>();
        
        foreach (var rental in rentals)
        {
            responseDtos.Add(MapToResponseDto(rental));
        }
        
        return responseDtos;
    }

    public async Task<IEnumerable<RentalResponseDto>> GetOverdueRentalsAsync()
    {
        var rentals = await _rentalRepository.GetOverdueRentalsAsync();
        var responseDtos = new List<RentalResponseDto>();
        
        foreach (var rental in rentals)
        {
            responseDtos.Add(MapToResponseDto(rental));
        }
        
        return responseDtos;
    }

    public async Task<bool> ConfirmRentalAsync(Guid id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null) return false;

        var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
        if (vehicle == null) return false;

        rental.ConfirmRental();
        vehicle.Rent();

        await _rentalRepository.UpdateAsync(rental);
        await _vehicleRepository.UpdateAsync(vehicle);
        return true;
    }

    public async Task<RentalResponseDto?> CompleteRentalAsync(Guid id, CompleteRentalDto dto)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null) return null;

        var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
        if (vehicle == null) return null;

        rental.CompleteRental(dto.FinalMileage, dto.Notes);
        vehicle.UpdateMileage(dto.FinalMileage);
        vehicle.ReturnFromRental();

        await _rentalRepository.UpdateAsync(rental);
        await _vehicleRepository.UpdateAsync(vehicle);
        
        return MapToResponseDto(rental);
    }

    public async Task<bool> CancelRentalAsync(Guid id, string reason)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null) return false;

        if (rental.Status == RentalStatus.Active)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId);
            if (vehicle != null)
            {
                vehicle.ReturnFromRental();
                await _vehicleRepository.UpdateAsync(vehicle);
            }
        }

        rental.CancelRental(reason);
        await _rentalRepository.UpdateAsync(rental);
        return true;
    }

    public async Task<RentalResponseDto?> ExtendRentalAsync(Guid id, ExtendRentalDto dto)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null) return null;

        rental.ExtendRental(dto.NewEndDate);
        await _rentalRepository.UpdateAsync(rental);
        
        return MapToResponseDto(rental);
    }

    public async Task<bool> MarkAsOverdueAsync(Guid id)
    {
        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null) return false;

        rental.MarkAsOverdue();
        await _rentalRepository.UpdateAsync(rental);
        return true;
    }

    private RentalResponseDto MapToResponseDto(Rental rental)
    {
        UserResponseDto? userDto = null;
        VehicleResponseDto? vehicleDto = null;

        if (rental.User != null)
        {
            userDto = new UserResponseDto(
                rental.User.Id,
                rental.User.Name?.FirstName ?? string.Empty,
                rental.User.Name?.LastName ?? string.Empty,
                rental.User.Email?.Value ?? string.Empty,
                rental.User.GetDisplayName(),
                rental.User.Document?.Value,
                rental.User.Document?.Type,
                rental.User.DriverLicense?.Number,
                rental.User.DriverLicense?.Category,
                rental.User.DriverLicense?.ExpiryDate,
                rental.User.DriverLicense?.IsValid() ?? false,
                rental.User.IsActive,
                rental.User.IsEligibleToRent,
                rental.User.CreatedAt,
                rental.User.UpdatedAt
            );
        }

        if (rental.Vehicle != null)
        {
            VehicleModelResponseDto? vehicleModelDto = null;
            if (rental.Vehicle.VehicleModel != null)
            {
                vehicleModelDto = new VehicleModelResponseDto(
                    rental.Vehicle.VehicleModel.Id,
                    rental.Vehicle.VehicleModel.Brand ?? string.Empty,
                    rental.Vehicle.VehicleModel.Model ?? string.Empty,
                    rental.Vehicle.VehicleModel.Year,
                    rental.Vehicle.VehicleModel.FuelType ?? string.Empty,
                    rental.Vehicle.VehicleModel.Engine ?? string.Empty,
                    rental.Vehicle.VehicleModel.DailyRate,
                    rental.Vehicle.VehicleModel.GetDisplayName(),
                    rental.Vehicle.VehicleModel.IsActive,
                    rental.Vehicle.VehicleModel.CreatedAt,
                    rental.Vehicle.VehicleModel.UpdatedAt
                );
            }

            vehicleDto = new VehicleResponseDto(
                rental.Vehicle.Id,
                rental.Vehicle.LicensePlate?.Value ?? string.Empty,
                rental.Vehicle.Chassis?.Value ?? string.Empty,
                rental.Vehicle.VehicleModelId,
                vehicleModelDto,
                rental.Vehicle.Type,
                rental.Vehicle.Status,
                rental.Vehicle.Mileage,
                rental.Vehicle.RestrictionReason,
                rental.Vehicle.GetStatusDescription(),
                rental.Vehicle.IsAvailableForRent(),
                rental.Vehicle.CreatedAt,
                rental.Vehicle.UpdatedAt
            );
        }

        return new RentalResponseDto(
            rental.Id,
            rental.UserId,
            userDto,
            rental.VehicleId,
            vehicleDto,
            rental.StartDate,
            rental.EndDate,
            rental.ActualReturnDate,
            rental.DailyRate,
            rental.TotalAmount,
            rental.FinalAmount,
            rental.Status,
            rental.GetStatusDescription(),
            rental.Notes,
            rental.FinalMileage,
            rental.GetDuration(),
            rental.GetActualDuration(),
            rental.IsOverdue(),
            rental.CreatedAt,
            rental.UpdatedAt
        );
    }
}