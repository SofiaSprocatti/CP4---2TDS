using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly ApplicationDbContext _context;

    public RentalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByVehicleIdAsync(Guid vehicleId)
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status)
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .Where(r => r.Status == RentalStatus.Active)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetOverdueRentalsAsync()
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .Where(r => r.Status == RentalStatus.Overdue || 
                       (r.Status == RentalStatus.Active && r.EndDate < DateTime.UtcNow))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Rental?> GetActiveRentalByVehicleIdAsync(Guid vehicleId)
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .FirstOrDefaultAsync(r => r.VehicleId == vehicleId && 
                               (r.Status == RentalStatus.Active || r.Status == RentalStatus.Pending));
    }

    public async Task<Rental?> GetActiveRentalByUserIdAsync(Guid userId)
    {
        return await _context.Rentals
            .Include(r => r.User)
            .Include(r => r.Vehicle)
                .ThenInclude(v => v!.VehicleModel)
            .FirstOrDefaultAsync(r => r.UserId == userId && 
                               (r.Status == RentalStatus.Active || r.Status == RentalStatus.Pending));
    }

    public async Task AddAsync(Rental rental)
    {
        await _context.Rentals.AddAsync(rental);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Rental rental)
    {
        _context.Rentals.Update(rental);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var rental = await GetByIdAsync(id);
        if (rental != null)
        {
            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();
        }
    }
}

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _context.Documents
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await _context.Documents
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Documents
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .Where(d => d.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByVehicleIdAsync(Guid vehicleId)
    {
        return await _context.Documents
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .Where(d => d.VehicleId == vehicleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByRentalIdAsync(Guid rentalId)
    {
        return await _context.Documents
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .Where(d => d.RentalId == rentalId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByTypeAsync(DocumentType type)
    {
        return await _context.Documents
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .Where(d => d.Type == type)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetExpiredDocumentsAsync()
    {
        return await _context.Documents
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .Where(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value <= DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task AddAsync(Document document)
    {
        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Document document)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var document = await GetByIdAsync(id);
        if (document != null)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}