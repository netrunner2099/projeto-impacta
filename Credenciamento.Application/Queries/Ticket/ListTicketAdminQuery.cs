namespace Credenciamento.Application.Queries.Ticket;

public class ListTicketAdminQuery : IRequest<ListTicketAdminQueryResponse>
{
    public long? EventId { get; set; }
    public byte? Status { get; set; }
    public byte? Payment { get; set; }
}
