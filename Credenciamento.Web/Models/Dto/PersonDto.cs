namespace Credenciamento.Web.Models.Dto;

public class PersonDto
{
    [JsonPropertyName("personId"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public long PersonId { get; set; }

    [JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Name { get; set; }

    [JsonPropertyName("document"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Document { get; set; }

    [JsonPropertyName("email"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Email { get; set; }

    [JsonPropertyName("phone"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Phone { get; set; }

    [JsonPropertyName("birthDay"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? BirthDay { get; set; }

    [JsonPropertyName("zipCode"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string ZipCode { get; set; }

    [JsonPropertyName("address"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Address { get; set; }

    [JsonPropertyName("number"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Number { get; set; }

    [JsonPropertyName("complement"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Complement { get; set; }

    [JsonPropertyName("neighborhood"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Neighborhood { get; set; }

    [JsonPropertyName("city"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string City { get; set; }

    [JsonPropertyName("state"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string State { get; set; }

    [JsonPropertyName("status"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte Status { get; set; }

    [JsonPropertyName("createdAt"), JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; set; }
}