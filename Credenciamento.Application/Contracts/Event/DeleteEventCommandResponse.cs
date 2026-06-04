namespace Credenciamento.Application.Contracts.Event;

public class DeleteEventCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
