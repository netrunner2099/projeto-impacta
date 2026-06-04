using Credenciamento.Application.Contracts.Ticket;
using Credenciamento.Application.Queries.Ticket;

namespace Credenciamento.Application.Handlers.Ticket;

public class ListTicketAdminQueryHandler : IRequestHandler<ListTicketAdminQuery, ListTicketAdminQueryResponse>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly ITicketRepository _repository;

    public ListTicketAdminQueryHandler(ILogger<ListTicketAdminQueryHandler> logger, IMapper mapper, ITicketRepository repository)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<ListTicketAdminQueryResponse> Handle(ListTicketAdminQuery request, CancellationToken cancellationToken)
    {
        ListTicketAdminQueryResponse returns = new();
        try
        {
            IEnumerable<Domain.Entities.Ticket> tickets;

            if (request.EventId.HasValue && request.EventId > 0)
                tickets = await _repository.ListByEventIdAsync(request.EventId.Value);
            else
                tickets = await _repository.ListAllAsync();

            if (request.Status.HasValue)
                tickets = tickets.Where(t => t.Status == request.Status.Value);

            if (request.Payment.HasValue)
                tickets = tickets.Where(t => t.Payment == request.Payment.Value);

            returns.Tickets = _mapper.Map<IEnumerable<TicketModel>>(tickets.OrderByDescending(t => t.CreatedAt));
            returns.Success = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
            returns.Message = $"Erro ao listar tickets: {ex.Message}";
        }
        return returns;
    }
}
