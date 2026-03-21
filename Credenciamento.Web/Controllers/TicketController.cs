using Credenciamento.Application.Queries.Ticket;
using Credenciamento.Web.Models;
using Credenciamento.Web.Models.Dto;
using Credenciamento.Web.Services;

namespace Credenciamento.Web.Controllers;

[Route("[controller]/[action]")]
public class TicketController : LocalControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    public TicketController(
        IMapper mapper,
        IMediator mediator,
        IServiceProvider services) : base(services)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Index(string id)
    {
        var model = _mapper.Map<TicketIndexViewModel>(GetLocalBaseViewModel());
        var result = await _mediator.Send(new GetTicketQuery { Transaction = id });
        model.Ticket = _mapper.Map<TicketDto>(result);
        return View(model);
    }
}
