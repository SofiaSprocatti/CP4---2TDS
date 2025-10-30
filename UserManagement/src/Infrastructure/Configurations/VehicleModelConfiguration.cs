using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Configurations;

public class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("VehicleModels");

        builder.HasKey(vm => vm.Id);

        builder.Property(vm => vm.Brand)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(vm => vm.Model)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(vm => vm.Year)
            .IsRequired();

        builder.Property(vm => vm.FuelType)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(vm => vm.Engine)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(vm => vm.DailyRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(vm => vm.IsActive)
            .IsRequired();

        builder.Property(vm => vm.CreatedAt)
            .IsRequired();

        builder.Property(vm => vm.UpdatedAt);

        builder.HasIndex(vm => new { vm.Brand, vm.Model, vm.Year })
            .HasDatabaseName("IX_VehicleModels_Brand_Model_Year");
    }
}