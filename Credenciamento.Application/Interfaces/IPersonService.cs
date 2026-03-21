namespace Credenciamento.Application.Interfaces;

public interface IPersonService
{
    Task<PersonModel> AddAsync(PersonModel model);
}
