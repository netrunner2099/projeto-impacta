namespace Credenciamento.Application.Contracts.User;

public class ActivateUserCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
