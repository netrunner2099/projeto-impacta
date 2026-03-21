namespace Credenciamento.Application.Validators.Ticket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    private readonly IPersonRepository _personRepository;
    private readonly IEventRepository _eventRepository;
    public CreateTicketCommandValidator(
        IPersonRepository personRepository,
        IEventRepository eventRepository)
    {
        _personRepository = personRepository;
        _eventRepository = eventRepository;

        // Validando a pessoa
        RuleFor(x => x.PersonId)
            .GreaterThan(0).WithMessage("PersonId obrigatório.")
            .MustAsync(async (personId, cancellation) =>
            {
                var person = await _personRepository.GetByIdAsync(personId);
                return person is not null;
            }).WithMessage("PersonId inválido."); 

        RuleFor(x => x.EventId)
            .GreaterThan(0).WithMessage("EventId é obrigatório.")
            .MustAsync(async (eventId, cancellation) =>
            {
                var eventEntity = await _eventRepository.GetByIdAsync(eventId);
                return eventEntity is not null;
            }).WithMessage("EventId inválido.");
    }

    public IEnumerable<string> ValidateCommand(CreateTicketCommand command)
    {
        var result = Validate(command);
        return result.Errors.Select(e => e.ErrorMessage);
    }
    public bool IsValid(CreateTicketCommand command)
    {
        return Validate(command).IsValid;
    }

}


