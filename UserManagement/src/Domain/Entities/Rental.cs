using UserManagement.Domain.Common;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities;

public class Rental : BaseEntity
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public Guid VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public DateTime? ActualReturnDate { get; private set; }
    public decimal DailyRate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal? FinalAmount { get; private set; }
    public RentalStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public int? FinalMileage { get; private set; }

    private readonly List<Document> _documents = new();
    public IReadOnlyList<Document> Documents => _documents.AsReadOnly();

    private Rental() { }

    private Rental(Guid userId, Guid vehicleId, DateTime startDate, DateTime endDate, decimal dailyRate) : base()
    {
        UserId = userId;
        VehicleId = vehicleId;
        StartDate = startDate;
        EndDate = endDate;
        DailyRate = dailyRate;
        Status = RentalStatus.Pending;
        CalculateTotalAmount();
    }

    public static Rental Create(Guid userId, Guid vehicleId, DateTime startDate, DateTime endDate, decimal dailyRate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("End date must be after start date");

        if (startDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Start date cannot be in the past");

        if (dailyRate <= 0)
            throw new ArgumentException("Daily rate must be greater than zero");

        return new Rental(userId, vehicleId, startDate, endDate, dailyRate);
    }

    public void ConfirmRental()
    {
        if (Status != RentalStatus.Pending)
            throw new InvalidOperationException("Only pending rentals can be confirmed");

        Status = RentalStatus.Active;
        SetUpdatedAt();
    }

    public void CompleteRental(int finalMileage, string? notes = null)
    {
        if (Status != RentalStatus.Active)
            throw new InvalidOperationException("Only active rentals can be completed");

        if (finalMileage < 0)
            throw new ArgumentException("Final mileage cannot be negative");

        ActualReturnDate = DateTime.UtcNow;
        FinalMileage = finalMileage;
        Notes = notes;
        Status = RentalStatus.Completed;
        CalculateFinalAmount();
        SetUpdatedAt();
    }

    public void CancelRental(string reason)
    {
        if (Status == RentalStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed rental");

        Status = RentalStatus.Cancelled;
        Notes = reason;
        SetUpdatedAt();
    }

    public void MarkAsOverdue()
    {
        if (Status != RentalStatus.Active)
            throw new InvalidOperationException("Only active rentals can be marked as overdue");

        if (DateTime.UtcNow.Date <= EndDate.Date)
            throw new InvalidOperationException("Rental is not overdue yet");

        Status = RentalStatus.Overdue;
        SetUpdatedAt();
    }

    public void ExtendRental(DateTime newEndDate)
    {
        if (Status != RentalStatus.Active && Status != RentalStatus.Overdue)
            throw new InvalidOperationException("Can only extend active or overdue rentals");

        if (newEndDate <= EndDate)
            throw new ArgumentException("New end date must be after current end date");

        EndDate = newEndDate;
        CalculateTotalAmount();
        
        if (Status == RentalStatus.Overdue && DateTime.UtcNow.Date <= newEndDate.Date)
        {
            Status = RentalStatus.Active;
        }
        
        SetUpdatedAt();
    }

    public void AddDocument(Document document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        document.AssignToRental(Id);
        _documents.Add(document);
        SetUpdatedAt();
    }

    private void CalculateTotalAmount()
    {
        var days = (EndDate.Date - StartDate.Date).Days;
        if (days <= 0) days = 1; 
        TotalAmount = days * DailyRate;
    }

    private void CalculateFinalAmount()
    {
        if (!ActualReturnDate.HasValue)
        {
            FinalAmount = TotalAmount;
            return;
        }

        var actualDays = (ActualReturnDate.Value.Date - StartDate.Date).Days;
        if (actualDays <= 0) actualDays = 1;

        var baseFinalAmount = actualDays * DailyRate;

        if (ActualReturnDate.Value.Date > EndDate.Date)
        {
            var lateDays = (ActualReturnDate.Value.Date - EndDate.Date).Days;
            var penalty = lateDays * DailyRate * 1.5m; 
            baseFinalAmount += penalty;
        }

        FinalAmount = baseFinalAmount;
    }

    public bool IsOverdue() => Status == RentalStatus.Active && DateTime.UtcNow.Date > EndDate.Date;

    public int GetDuration() => (EndDate.Date - StartDate.Date).Days;

    public int? GetActualDuration() => ActualReturnDate.HasValue 
        ? (ActualReturnDate.Value.Date - StartDate.Date).Days 
        : null;

    public string GetStatusDescription()
    {
        return Status switch
        {
            RentalStatus.Pending => "Pending Confirmation",
            RentalStatus.Active => "Active",
            RentalStatus.Completed => "Completed",
            RentalStatus.Cancelled => "Cancelled",
            RentalStatus.Overdue => "Overdue",
            _ => "Unknown"
        };
    }
}