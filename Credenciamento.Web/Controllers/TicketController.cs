using Credenciamento.Application.Queries.Ticket;
using Credenciamento.Web.Models.Dto;

namespace Credenciamento.Web.Controllers;

[Route("[controller]/[action]")]
public class TicketController : LocalControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public TicketController(
        IMediator mediator,
        IMapper mapper,
        ILogger<TicketController> logger,
        IServiceProvider services) : base(services)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Index(string id)
    {
        _logger.LogInformation("TicketController.Index chamado com id: {Id}", id);

        var model = _mapper.Map<TicketIndexViewModel>(GetLocalBaseViewModel());

        try
        {
            var query = new GetTicketQuery { Transaction = id };
            var result = await _mediator.Send(query);

            if (result != null)
            {
                _logger.LogInformation("Ticket encontrado: {TicketId}", result.TicketId);
                model.Ticket = _mapper.Map<TicketDto>(result);
            }
            else
            {
                _logger.LogWarning("Ticket não encontrado para Transaction: {Transaction}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar ticket {Transaction}", id);
        }

        return View(model);
    }

    [HttpGet("{transaction}")]
    public async Task<IActionResult> GetQRCode(string transaction)
    {
        _logger.LogInformation("GetQRCode chamado para transação: {Transaction}", transaction);

        try
        {
            var query = new GetTicketQuery { Transaction = transaction };
            var result = await _mediator.Send(query);

            if (result?.QRCodeResponse != null)
            {
                _logger.LogInformation("QR Code encontrado para transação: {Transaction}", transaction);
                return Json(new
                {
                    qrCodeDataUrl = result.QRCodeResponse.DataUrl,
                    transaction = result.Transaction
                });
            }

            _logger.LogWarning("QR Code não encontrado para transação: {Transaction}", transaction);
            return NotFound(new { message = "QR Code não encontrado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar QR Code para transação {Transaction}", transaction);
            return StatusCode(500, new { message = "Erro ao carregar QR Code" });
        }
    }
}