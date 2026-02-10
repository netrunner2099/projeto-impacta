namespace Credenciamento.Application.Contracts.Person;

public class CreatePersonCommandResponse : PersonModel
{
    public IEnumerable<string> Errors { get; set; }
}


