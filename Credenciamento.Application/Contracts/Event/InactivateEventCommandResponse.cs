namespace Credenciamento.Application.Contracts.Event;

public class InactivateEventCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
