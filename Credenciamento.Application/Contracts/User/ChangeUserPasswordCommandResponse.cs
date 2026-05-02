namespace Credenciamento.Application.Contracts.User;

public class ChangeUserPasswordCommandResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; }
}
