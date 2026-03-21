namespace Credenciamento.Application.Queries.User;

public class GetOtpCodeQuery : IRequest<GetOtpCodeQueryResponse>
{
    public string Email { get; set; }
}


