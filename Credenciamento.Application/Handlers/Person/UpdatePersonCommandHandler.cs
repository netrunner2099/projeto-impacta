using Credenciamento.Application.Commands.Person;
using Credenciamento.Application.Contracts.Person;
using Credenciamento.Shared.Extensions;

namespace Credenciamento.Application.Handlers.Person;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, UpdatePersonCommandResponse>
{
    private readonly ILogger _logger;
    private readonly IPersonRepository _repository;

    public UpdatePersonCommandHandler(
        ILogger<UpdatePersonCommandHandler> logger,
        IPersonRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<UpdatePersonCommandResponse> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        UpdatePersonCommandResponse response = new();

        try
        {
            var person = await _repository.GetByIdAsync(request.PersonId);
            if (person is null)
            {
                response.Message = "Dados pessoais não encontrados.";
                return response;
            }

            person.Name = request.Name.ToTitleCase();
            person.Document = request.Document.MaskRemove();
            person.Email = request.Email.ToLower();
            person.Phone = request.Phone;
            person.BirthDay = request.BirthDay ?? person.BirthDay;
            person.ZipCode = request.ZipCode.MaskRemove();
            person.Address = request.Address.ToTitleCase();
            person.Number = request.Number;
            person.Complement = request.Complement;
            person.Neighborhood = request.Neighborhood.ToTitleCase();
            person.City = request.City.ToTitleCase();
            person.State = request.State.ToUpper();
            person.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(person);
            response.Success = true;
            response.Message = "Dados pessoais atualizados com sucesso.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            response.Message = "Erro ao atualizar dados pessoais.";
        }

        return response;
    }
}
