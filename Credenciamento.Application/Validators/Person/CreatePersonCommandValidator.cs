namespace Credenciamento.Application.Validators.Person;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    private readonly IPersonRepository _repository;
    private readonly IMapper _mapper;
    public CreatePersonCommandValidator(
        IPersonRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome não deve exceder 100 characters.");
     
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Formato de email inválido.");
        
        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Matches(@"^\d{11}$").WithMessage("CPF deve conter 11 dígitos.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("CEP é obrigatório.")
            .Matches(@"^\d{5}-\d{3}$").WithMessage("CEP deve estar no formato 12345-678.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório.")
            .Matches(@"^\(\d{2}\) \d{4,5}-\d{4}$").WithMessage("Telefone deve estar no formato (XX) XXXXX-XXXX.");

        RuleFor(x => x)
            .MustAsync(async (x, cancellation) => await CheckEmailAsync(x)).WithMessage("Email já está em uso.")
            .MustAsync(async (x, cancellation) => await CheckDocumentAsync(x)).WithMessage("Cpf já cadastrado.")
            .MustAsync(async (x, cancellation) => await CheckPhoneNumberAsync(x)).WithMessage("Telefone já cadastrado.");
    }

    public IEnumerable<string> ValidateCommand(CreatePersonCommand command)
    {
        var result = Validate(command);
        return result.Errors.Select(e => e.ErrorMessage);
    }
    public bool IsValid(CreatePersonCommand command)
    {
        return Validate(command).IsValid;
    }

    private async Task<bool> CheckEmailAsync(CreatePersonCommand command)
    {
        return !(await _repository.EmailExistsAsync(_mapper.Map<Domain.Entities.Person>(command)));
    }

    private async Task<bool> CheckDocumentAsync(CreatePersonCommand command)
    {
        return (!await _repository.DocumentExistsAsync(_mapper.Map<Domain.Entities.Person>(command)));
    }

    private async Task<bool> CheckPhoneNumberAsync(CreatePersonCommand command)
    {
        return (!await _repository.PhoneNumberExistsAsync(_mapper.Map<Domain.Entities.Person>(command)));
    }
}


