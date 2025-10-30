using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories;

public class VehicleModelRepository : IVehicleModelRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleModel?> GetByIdAsync(Guid id)
    {
        return await _context.VehicleModels.FirstOrDefaultAsync(vm => vm.Id == id);
    }

    public async Task<IEnumerable<VehicleModel>> GetAllAsync()
    {
        return await _context.VehicleModels.ToListAsync();
    }

    public async Task<IEnumerable<VehicleModel>> GetActiveModelsAsync()
    {
        return await _context.VehicleModels.Where(vm => vm.IsActive).ToListAsync();
    }

    public async Task AddAsync(VehicleModel vehicleModel)
    {
        await _context.VehicleModels.AddAsync(vehicleModel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(VehicleModel vehicleModel)
    {
        _context.VehicleModels.Update(vehicleModel);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var vehicleModel = await GetByIdAsync(id);
        if (vehicleModel != null)
        {
            _context.VehicleModels.Remove(vehicleModel);
            await _context.SaveChangesAsync();
        }
    }
}

public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _context.Vehicles
            .Include(v => v.VehicleModel)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _context.Vehicles
            .Include(v => v.VehicleModel)
            .FirstOrDefaultAsync(v => v.LicensePlate != null && v.LicensePlate.Value == licensePlate);
    }

    public async Task<Vehicle?> GetByChassisAsync(string chassis)
    {
        return await _context.Vehicles
            .Include(v => v.VehicleModel)
            .FirstOrDefaultAsync(v => v.Chassis != null && v.Chassis.Value == chassis);
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles
            .Include(v => v.VehicleModel)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status)
    {
        return await _context.Vehicles
            .Include(v => v.VehicleModel)
            .Where(v => v.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
    {
        return await _context.Vehicles
            .Include(v => v.VehicleModel)
            .Where(v => v.Status == VehicleStatus.Available)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetByTypeAsync(VehicleType type)
    {
        return await _context.Vehicles
            .Include(v => v.VehicleModel)
            .Where(v => v.Type == type)
            .ToListAsync();
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var vehicle = await GetByIdAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByLicensePlateAsync(string licensePlate)
    {
        return await _context.Vehicles.AnyAsync(v => v.LicensePlate != null && v.LicensePlate.Value == licensePlate);
    }

    public async Task<bool> ExistsByChassisAsync(string chassis)
    {
        return await _context.Vehicles.AnyAsync(v => v.Chassis != null && v.Chassis.Value == chassis);
    }
}