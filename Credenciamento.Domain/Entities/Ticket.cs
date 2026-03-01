namespace Credenciamento.Domain.Entities;

public class Ticket
{
    public long TicketId { get; set; }
    public long PersonId { get; set; }
    public long EventId { get; set; }
    public decimal Price { get; set; }
    public byte Payment { get; set; }
    public string Transaction { get; set; }
    public string Auth { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Person Person { get; set; }
    public Event Event { get; set; }
}