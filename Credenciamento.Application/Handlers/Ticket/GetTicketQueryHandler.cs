using Credenciamento.Application.Queries.Ticket;
using Credenciamento.Application.Services.QrCode;

namespace Credenciamento.Application.Handlers.Ticket;

public class GetTicketQueryHandler : IRequestHandler<GetTicketQuery, GetTicketQueryResponse?>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly ITicketRepository _repository;
    private readonly IQrCodeClient _qrcode;
    private readonly string baseUrl;
    public GetTicketQueryHandler(
        ILogger<GetTicketQueryHandler> logger,
        IMapper mapper,
        ITicketRepository repository, 
        IQrCodeClient qrcode,
        IConfiguration configuration)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _qrcode = qrcode;
        baseUrl = $"{configuration["Environment:BaseUrl"]}/ticket/index/{{0}}";
    }

    public async Task<GetTicketQueryResponse> Handle(GetTicketQuery request, CancellationToken cancellationToken)
    {
        GetTicketQueryResponse returns = null;
        try
        {
            var ticket = await _repository.GetByIdAsync(request.TicketId ?? 0);
            if (ticket is null)
                ticket = await _repository.GetByTransactionAsync(request.Transaction);

            if(ticket is null)
            {
                _logger.LogWarning("Handle: Ticket not found. TicketId: {0}, Transaction: {1}", request.TicketId, request.Transaction);
                return null;
            }

            var url = string.Format(baseUrl, ticket.Transaction);
            var qrcode = await _qrcode.GenerateAsync(new QrCodeRequest { Data = url });
            returns = _mapper.Map<GetTicketQueryResponse>(ticket);
            returns.QRCodeResponse = qrcode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle: {0}", ex.Message);
        }

        return returns;
    }
}


