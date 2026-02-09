namespace Credenciamento.Infrastructure.EntityConfiguration;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("person", "dbo");

        builder.HasKey(p => p.PersonId);

        builder.Property(p => p.PersonId)
            .HasColumnName("personid")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(255)");

        builder.Property(p => p.Document)
            .HasColumnName("document")
            .HasColumnType("varchar(12)");

        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasColumnType("varchar(255)");

        builder.Property(p => p.Phone)
            .HasColumnName("phone")
            .HasColumnType("varchar(20)");

        builder.Property(p => p.ZipCode)
            .HasColumnName("zipcode")
            .HasColumnType("varchar(9)");

        builder.Property(p => p.Address)
            .HasColumnName("address")
            .HasColumnType("varchar(255)");

        builder.Property(p => p.Number)
            .HasColumnName("number")
            .HasColumnType("varchar(50)");

        builder.Property(p => p.Complement)
            .HasColumnName("complement")
            .HasColumnType("varchar(100)");

        builder.Property(p => p.Neighborhood)
            .HasColumnName("neighborhood")
            .HasColumnType("varchar(100)");
            
        builder.Property(p => p.City)
            .HasColumnName("city")
            .HasColumnType("varchar(100)");

        builder.Property(p => p.State)
            .HasColumnName("state")
            .HasColumnType("varchar(2)");

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasColumnType("tinyint");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("createdat")
            .HasColumnType("datetime");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updatedat")
            .HasColumnType("datetime");

        builder.HasMany(m => m.Tickets)
            .WithOne(t => t.Person)
            .HasForeignKey(t => t.PersonId);
    }
}