namespace Credenciamento.Domain.Entities;

public class Event
{
    public long EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
    public decimal Price { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public IEnumerable<Ticket> Tickets { get; set; }
}