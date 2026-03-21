namespace Credenciamento.Application.Queries.Person;

public class GetPersonQuery : IRequest<GetPersonQueryResponse?>
{
    public long PersonId { get; set; }
}


