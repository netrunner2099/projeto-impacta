namespace Credenciamento.Infrastructure.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user", "dbo");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnType("bigint")
            .HasColumnName("userid")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.PersonId)
            .HasColumnName("personid")
            .HasColumnType("bigint");

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(255)")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasColumnType("varchar(255)");

        builder.Property(u => u.Password)
            .HasColumnName("password")
            .HasColumnType("varchar(255)");

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasColumnType("tinyint");

        builder.Property(u => u.Status)
            .HasColumnName("status")
            .HasColumnType("tinyint");

        builder.Property(u => u.CreatedAt)
            .HasColumnName("createdat")
            .HasColumnType("datetime");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updatedat")
            .HasColumnType("datetime");
    }
}