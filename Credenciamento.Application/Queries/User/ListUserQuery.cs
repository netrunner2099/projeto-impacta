namespace Credenciamento.Application.Queries.User;

public class ListUserQuery : IRequest<ListUserQueryResponse>
{
    public string? Name { get; set; }
    public byte? Role { get; set; }
}
