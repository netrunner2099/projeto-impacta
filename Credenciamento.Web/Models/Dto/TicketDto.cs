using Credenciamento.Application.Models;
using Credenciamento.Domain.Enums;

namespace Credenciamento.Web.Models.Dto;

public class TicketDto
{
    [JsonPropertyName("ticketId"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public long? TicketId { get; set; }

    [JsonPropertyName("personId"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public long? PersonId { get; set; }

    [JsonPropertyName("eventId"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public long? EventId { get; set; }

    [JsonPropertyName("price"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public decimal? Price { get; set; }

    [JsonPropertyName("payment"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte? Payment { get; set; }

    [JsonPropertyName("paymentName"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? PaymentName { 
        get 
        {
            return Payment.HasValue ? ((TicketPayment)Payment.Value).GetDescription() : null;
        } 
    }

    [JsonPropertyName("status"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte? Status { get; set; }

    [JsonPropertyName("transaction"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? Transaction { get; set; }

    [JsonPropertyName("auth"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? Auth { get; set; }

    [JsonPropertyName("qrcode"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public QRCodeResponse QRCodeResponse { get; set; }

    [JsonPropertyName("event"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public EventDto Event { get; set; }

    [JsonPropertyName("person"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public PersonDto Person { get; set; }
}
