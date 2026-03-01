using Credenciamento.Domain.Enums;

namespace Credenciamento.Application.Handlers.Ticket;

public class PayTicketCommandHandler : IRequestHandler<PayTicketCommand, PayTicketCommandResponse>
{
    public readonly ILogger _logger;
    public readonly IMapper _mapper;
    public readonly ITicketRepository _repository;
    public PayTicketCommandHandler(
        ILogger<PayTicketCommandHandler> logger,
        IMapper mapper,
        ITicketRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<PayTicketCommandResponse> Handle(PayTicketCommand request, CancellationToken cancellationToken)
    {
        PayTicketCommandResponse returns = null;

        try
        {
            var ticket = await _repository.GetByIdAsync(request.TicketId);
            if(ticket is null)
            {
                _logger.LogError("Handle: Ticket not found with id {0}", request.TicketId);
                return null;
            }

            ticket.Transaction = $"{Guid.NewGuid()}";
            ticket.Auth = StringHelpers.ToBase64($"{ticket.TicketId}-{ticket.Price}-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}-{ticket.Transaction}");
            ticket.Status = (byte)TicketStatus.Paid;
            ticket.UpdatedAt = DateTime.UtcNow;
            var result = await _repository.UpdateAsync(ticket);
            if(result is null)
            {
                _logger.LogError("Handle: Ticket not updated with id {0}", request.TicketId);
                return null;
            }

            return _mapper.Map<PayTicketCommandResponse>(result);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return returns;
    }
}


