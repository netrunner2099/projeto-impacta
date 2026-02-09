using Credenciamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credenciamento.Infrastructure.EntityConfiguration
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("event", "dbo");

            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId)
                .HasColumnName("eventid")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.Begin)
                .HasColumnName("begin")
                .IsRequired();

            builder.Property(e => e.End)
                .HasColumnName("end")
                .IsRequired();

            builder.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("numeric(18,6)")
                .IsRequired();

            builder.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue((byte)1)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnName("createdat")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updatedat");
        }
    }
}