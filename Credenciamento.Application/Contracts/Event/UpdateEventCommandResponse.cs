namespace Credenciamento.Application.Contracts.Event;

public class UpdateEventCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string[] Errors { get; set; } = [];
}
