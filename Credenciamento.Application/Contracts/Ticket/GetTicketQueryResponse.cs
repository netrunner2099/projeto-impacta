namespace Credenciamento.Application.Contracts.Ticket;

public class GetTicketQueryResponse : TicketModel
{
    public QRCodeResponse QRCodeResponse { get; set; } = new QRCodeResponse();
}


