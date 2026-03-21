using Credenciamento.Application.Queries.Ticket;
using Credenciamento.Web.Models;
using Credenciamento.Web.Models.Dto;

namespace Credenciamento.Web.Controllers;

[Route("[controller]/[action]")]
public class TicketController : Controller
{

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    public TicketController(
        ILogger<TicketController> logger,
        IMapper mapper,
        IMediator mediator)
    {
        _logger = logger;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Index(string id)
    {
        var model = new TicketIndexViewModel();
        var result = await _mediator.Send(new GetTicketQuery { Transaction = id });
        model.Ticket = _mapper.Map<TicketDto>(result);
        return View(model);
    }
}
