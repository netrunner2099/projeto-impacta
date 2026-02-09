using Credenciamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credenciamento.Infrastructure.EntityConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user", "dbo");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserId)
                .HasColumnName("userid")
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Password)
                .HasColumnName("password")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Role)
                .HasColumnName("role")
                .HasDefaultValue((byte)1)
                .IsRequired();

            builder.Property(u => u.Status)
                .HasColumnName("status")
                .HasDefaultValue((byte)1)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasColumnName("createdat")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .HasColumnName("updatedat");

            // Index for email uniqueness
            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}