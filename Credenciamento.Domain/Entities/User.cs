namespace Credenciamento.Domain.Entities;

public class User
{
    public long UserId { get; set; }
    public long? PersonId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public byte Role { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}