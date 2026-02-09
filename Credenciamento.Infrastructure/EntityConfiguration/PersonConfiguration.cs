using Credenciamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credenciamento.Infrastructure.EntityConfiguration
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("person", "dbo");

            builder.HasKey(p => p.PersonId);

            builder.Property(p => p.PersonId)
                .HasColumnName("personid")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.Document)
                .HasColumnName("document")
                .HasMaxLength(12)
                .IsRequired();

            builder.Property(p => p.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.ZipCode)
                .HasColumnName("zipcode")
                .HasMaxLength(9)
                .IsRequired();

            builder.Property(p => p.Address)
                .HasColumnName("address")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.Number)
                .HasColumnName("number")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Complement)
                .HasColumnName("complement")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Neighborhood)
                .HasColumnName("neighborhood")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.City)
                .HasColumnName("city")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.State)
                .HasColumnName("state")
                .HasMaxLength(2)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasColumnName("status")
                .HasDefaultValue((byte)1)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnName("createdat")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("updatedat");
        }
    }
}