namespace Credenciamento.Application.Contracts.User;

public class CreateUserCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = [];
}
