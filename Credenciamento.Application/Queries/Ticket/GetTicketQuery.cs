namespace Credenciamento.Application.Queries.Ticket;

public class GetTicketQuery : IRequest<GetTicketQueryResponse?>
{
    public long? TicketId { get; set; }
    public string? Transaction { get; set; }
}


