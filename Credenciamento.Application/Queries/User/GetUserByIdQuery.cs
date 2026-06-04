namespace Credenciamento.Application.Queries.User;

public class GetUserByIdQuery : IRequest<GetUserByIdQueryResponse>
{
    public long UserId { get; set; }
}
