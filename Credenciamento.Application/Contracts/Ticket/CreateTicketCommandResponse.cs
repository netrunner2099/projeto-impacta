namespace Credenciamento.Application.Contracts.Ticket;

public class CreateTicketCommandResponse : TicketModel
{
    public IEnumerable<string> ErrorMessages { get; set; }
}


