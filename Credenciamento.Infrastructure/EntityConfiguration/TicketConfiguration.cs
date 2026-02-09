namespace Credenciamento.Infrastructure.EntityConfiguration;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("ticket", "dbo");

        builder.HasKey(t => t.TicketId);

        builder.Property(t => t.TicketId)
            .HasColumnName("ticketid")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.PersonId)
            .HasColumnName("personid")
            .HasColumnType("bigint");

        builder.Property(t => t.EventId)
            .HasColumnName("eventid")
            .HasColumnType("bigint");

        builder.Property(t => t.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(18,6)");

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasColumnType("tinyint");

        builder.Property(t => t.CreatedAt)
            .HasColumnName("createdat")
            .HasColumnType("datetime");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updatedat");

        // Foreign Keys
        builder.HasOne(t => t.Person)
            .WithMany(m => m.Tickets)
            .HasForeignKey(t => t.PersonId);

        builder.HasOne(t => t.Event)
            .WithMany(m => m.Tickets)
            .HasForeignKey(t => t.EventId);
    }
}