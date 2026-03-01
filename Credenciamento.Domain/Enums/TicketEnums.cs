using System.ComponentModel;

namespace Credenciamento.Domain.Enums;

public enum TicketStatus : byte 
{     
    Created = 0,
    Paid = 1,
    Canceled = 2,
    Deleted = 9
}


public enum TicketPayment : byte
{
    [Description("QRCode Pix")]
    Pix = 1,

    [Description("Cartão de Crédito")]
    CreditCard = 2,

    [Description("Boleto Bancário")]
    Boleto = 3
}