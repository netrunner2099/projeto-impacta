namespace Credenciamento.Application.Contracts.Ticket;

public class GetTicketQueryResponse : TicketModel
{
    public QRCodeResponse QRCodeResponse { get; set; } = new QRCodeResponse();
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}


