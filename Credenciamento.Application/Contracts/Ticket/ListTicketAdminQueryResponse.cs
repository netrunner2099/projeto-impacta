namespace Credenciamento.Application.Contracts.Ticket;

public class ListTicketAdminQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<TicketModel> Tickets { get; set; } = [];
}
