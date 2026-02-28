namespace Credenciamento.Application.Interfaces.Person;

public interface IPersonService
{
    Task<PersonModel> AddAsync(PersonModel model);
}
