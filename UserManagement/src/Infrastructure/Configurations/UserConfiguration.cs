using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsRequired();

        builder.OwnsOne(u => u.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(50)
                .IsRequired();

            nameBuilder.Property(n => n.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.OwnsOne(u => u.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();

            emailBuilder.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
        });

        builder.OwnsOne(u => u.Document, documentBuilder =>
        {
            documentBuilder.Property(d => d.Value)
                .HasColumnName("DocumentValue")
                .HasMaxLength(20);

            documentBuilder.Property(d => d.Type)
                .HasColumnName("DocumentType")
                .HasMaxLength(10);
        });

        builder.OwnsOne(u => u.DriverLicense, cnhBuilder =>
        {
            cnhBuilder.Property(c => c.Number)
                .HasColumnName("CNHNumber")
                .HasMaxLength(11);

            cnhBuilder.Property(c => c.ExpiryDate)
                .HasColumnName("CNHExpiryDate");

            cnhBuilder.Property(c => c.Category)
                .HasColumnName("CNHCategory")
                .HasMaxLength(5);
        });

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.IsEligibleToRent)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Ignore(u => u.Documents);
    }
}