using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.Number)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.FilePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.ExpiryDate);

        builder.Property(d => d.UserId);

        builder.Property(d => d.VehicleId);

        builder.Property(d => d.RentalId);

        builder.Property(d => d.IsActive)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt);

        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Vehicle)
            .WithMany()
            .HasForeignKey(d => d.VehicleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(d => d.Type)
            .HasDatabaseName("IX_Documents_Type");

        builder.HasIndex(d => d.Number)
            .HasDatabaseName("IX_Documents_Number");
    }
}