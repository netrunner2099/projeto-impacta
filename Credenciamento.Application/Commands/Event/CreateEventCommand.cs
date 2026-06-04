namespace Credenciamento.Application.Commands.Event;

public class CreateEventCommand : IRequest<CreateEventCommandResponse>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Local { get; set; }
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
    public decimal Price { get; set; }
}
