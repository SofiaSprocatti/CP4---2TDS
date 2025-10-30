using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Repositories;

public interface IVehicleModelRepository
{
    Task<VehicleModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<VehicleModel>> GetAllAsync();
    Task<IEnumerable<VehicleModel>> GetActiveModelsAsync();
    Task AddAsync(VehicleModel vehicleModel);
    Task UpdateAsync(VehicleModel vehicleModel);
    Task DeleteAsync(Guid id);
}

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
    Task<Vehicle?> GetByChassisAsync(string chassis);
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status);
    Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
    Task<IEnumerable<Vehicle>> GetByTypeAsync(VehicleType type);
    Task AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsByLicensePlateAsync(string licensePlate);
    Task<bool> ExistsByChassisAsync(string chassis);
}

public interface IRentalRepository
{
    Task<Rental?> GetByIdAsync(Guid id);
    Task<IEnumerable<Rental>> GetAllAsync();
    Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Rental>> GetByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status);
    Task<IEnumerable<Rental>> GetActiveRentalsAsync();
    Task<IEnumerable<Rental>> GetOverdueRentalsAsync();
    Task<Rental?> GetActiveRentalByVehicleIdAsync(Guid vehicleId);
    Task<Rental?> GetActiveRentalByUserIdAsync(Guid userId);
    Task AddAsync(Rental rental);
    Task UpdateAsync(Rental rental);
    Task DeleteAsync(Guid id);
}

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id);
    Task<IEnumerable<Document>> GetAllAsync();
    Task<IEnumerable<Document>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Document>> GetByVehicleIdAsync(Guid vehicleId);
    Task<IEnumerable<Document>> GetByRentalIdAsync(Guid rentalId);
    Task<IEnumerable<Document>> GetByTypeAsync(DocumentType type);
    Task<IEnumerable<Document>> GetExpiredDocumentsAsync();
    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Guid id);
}