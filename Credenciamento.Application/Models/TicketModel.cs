namespace Credenciamento.Application.Models;


public class TicketModel
{
    public long TicketId { get; set; }
    public long PersonId { get; set; }
    public long EventId { get; set; }
    public decimal Price { get; set; }
    public byte Status { get; set; }
    public string Auth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public PersonModel Person { get; set; }
    public EventModel Event { get; set; }
}