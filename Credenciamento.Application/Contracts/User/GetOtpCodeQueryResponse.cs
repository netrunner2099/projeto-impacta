namespace Credenciamento.Application.Contracts.User;

public class GetOtpCodeQueryResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}


