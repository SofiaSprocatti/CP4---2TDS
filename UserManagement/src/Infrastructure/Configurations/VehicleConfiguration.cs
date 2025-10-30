using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.OwnsOne(v => v.LicensePlate, plateBuilder =>
        {
            plateBuilder.Property(p => p.Value)
                .HasColumnName("LicensePlate")
                .HasMaxLength(10)
                .IsRequired();

            plateBuilder.HasIndex(p => p.Value)
                .IsUnique()
                .HasDatabaseName("IX_Vehicles_LicensePlate");
        });

        builder.OwnsOne(v => v.Chassis, chassisBuilder =>
        {
            chassisBuilder.Property(c => c.Value)
                .HasColumnName("Chassis")
                .HasMaxLength(17)
                .IsRequired();

            chassisBuilder.HasIndex(c => c.Value)
                .IsUnique()
                .HasDatabaseName("IX_Vehicles_Chassis");
        });

        builder.Property(v => v.VehicleModelId)
            .IsRequired();

        builder.Property(v => v.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.Mileage)
            .IsRequired();

        builder.Property(v => v.RestrictionReason)
            .HasMaxLength(500);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt);

        builder.HasOne(v => v.VehicleModel)
            .WithMany()
            .HasForeignKey(v => v.VehicleModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.Status)
            .HasDatabaseName("IX_Vehicles_Status");

        builder.HasIndex(v => v.Type)
            .HasDatabaseName("IX_Vehicles_Type");
    }
}