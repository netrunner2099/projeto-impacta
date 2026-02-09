using Credenciamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credenciamento.Infrastructure.EntityConfiguration
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("ticket", "dbo");

            builder.HasKey(t => t.TicketId);

            builder.Property(t => t.TicketId)
                .HasColumnName("ticketid")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.PersonId)
                .HasColumnName("personid")
                .IsRequired();

            builder.Property(t => t.EventId)
                .HasColumnName("eventid")
                .IsRequired();

            builder.Property(t => t.Price)
                .HasColumnName("price")
                .HasColumnType("numeric(18,6)")
                .IsRequired();

            builder.Property(t => t.Status)
                .HasColumnName("status")
                .HasDefaultValue((byte)1)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .HasColumnName("createdat")
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .HasColumnName("updatedat");

            // Foreign Keys
            builder.HasOne(t => t.Person)
                .WithMany()
                .HasForeignKey(t => t.PersonId)
                .HasConstraintName("fk_ticket_person")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Event)
                .WithMany()
                .HasForeignKey(t => t.EventId)
                .HasConstraintName("fk_ticket_event")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}