namespace Credenciamento.Web.Models.Dto;

public class EventDto
{
    [JsonPropertyName("eventId"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public long EventId { get; set; }

    [JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Name { get; set; }

    [JsonPropertyName("description"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    [JsonPropertyName("local"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Local { get; set; }

    [JsonPropertyName("begin"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DateTime Begin { get; set; }

    [JsonPropertyName("end"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DateTime End { get; set; }

    [JsonPropertyName("price"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public decimal Price { get; set; }

    [JsonPropertyName("status"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte Status { get; set; }

    [JsonPropertyName("createdAt"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; set; }
}
