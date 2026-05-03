namespace Credenciamento.Application.Contracts.User;

public class ResetUserPasswordCommandResponse
{
    public bool Success { get; set; } = false;
    public IEnumerable<string> Errors { get; set; }
}
