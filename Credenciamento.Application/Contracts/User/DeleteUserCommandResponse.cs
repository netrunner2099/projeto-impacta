namespace Credenciamento.Application.Contracts.User;

public class DeleteUserCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
