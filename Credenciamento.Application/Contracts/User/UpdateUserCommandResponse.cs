namespace Credenciamento.Application.Contracts.User;

public class UpdateUserCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = [];
}
