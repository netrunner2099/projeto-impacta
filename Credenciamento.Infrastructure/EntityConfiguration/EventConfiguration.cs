namespace Credenciamento.Infrastructure.EntityConfiguration;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("event", "dbo");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .HasColumnName("eventid")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(100)");

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasColumnType("varchar(500)");

        builder.Property(e => e.Local)
            .HasColumnName("local")
            .HasColumnType("varchar(200)");

        builder.Property(e => e.Begin)
            .HasColumnName("begin")
            .HasColumnType("datetime");

        builder.Property(e => e.End)
            .HasColumnName("end")
            .HasColumnType("datetime");

        builder.Property(e => e.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(18,6)");

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasColumnType("tinyint");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("createdat")
            .HasColumnType("datetime");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updatedat")
            .HasColumnType("datetime");

        builder.HasMany(m => m.Tickets)
            .WithOne(t => t.Event)
            .HasForeignKey(t => t.EventId);
    }
}