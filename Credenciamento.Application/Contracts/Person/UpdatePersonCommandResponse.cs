namespace Credenciamento.Application.Contracts.Person;

public class UpdatePersonCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
