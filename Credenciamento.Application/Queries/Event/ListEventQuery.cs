namespace Credenciamento.Application.Queries.Event;

public class ListEventQuery : IRequest<ListEventQueryResponse>
{
    public string? Name { get; set; }
    public byte? Status { get; set; }
    public bool OnlyFuture { get; set; } = true;
}
