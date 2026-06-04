namespace Credenciamento.Application.Contracts.User;

public class ListUserQueryResponse
{
    public IEnumerable<UserModel> Users { get; set; } = [];
}
