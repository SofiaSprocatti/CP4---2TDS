using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("Rentals");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.VehicleId)
            .IsRequired();

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.EndDate)
            .IsRequired();

        builder.Property(r => r.ActualReturnDate);

        builder.Property(r => r.DailyRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.FinalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.FinalMileage);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Vehicle)
            .WithMany()
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Rentals_Status");

        builder.HasIndex(r => r.StartDate)
            .HasDatabaseName("IX_Rentals_StartDate");

        builder.HasIndex(r => r.EndDate)
            .HasDatabaseName("IX_Rentals_EndDate");

        builder.Ignore(r => r.Documents);
    }
}