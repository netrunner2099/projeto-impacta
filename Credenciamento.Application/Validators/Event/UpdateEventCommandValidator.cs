using Credenciamento.Application.Commands.Event;
using FluentValidation;

namespace Credenciamento.Application.Validators.Event;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.EventId)
            .GreaterThan(0).WithMessage("ID do evento inválido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Begin)
            .NotEmpty().WithMessage("Data de início é obrigatória.");

        RuleFor(x => x.End)
            .NotEmpty().WithMessage("Data de término é obrigatória.")
            .GreaterThan(x => x.Begin).WithMessage("Data de término deve ser após a data de início.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Preço não pode ser negativo.");
    }
}
