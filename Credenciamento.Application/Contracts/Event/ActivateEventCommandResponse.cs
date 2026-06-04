namespace Credenciamento.Application.Contracts.Event;

public class ActivateEventCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
