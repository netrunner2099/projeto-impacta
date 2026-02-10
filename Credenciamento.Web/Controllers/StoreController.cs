using Credenciamento.Application.Models;
using Credenciamento.Application.Queries.Event;
using Credenciamento.Web.Models;

namespace Credenciamento.Web.Controllers;

public class StoreController : Controller
{
    private readonly IMediator _mediator;
    public StoreController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(int id)
    {
        var model = new StoreIndexViewModel();
        var result = await _mediator.Send(new GetEventQuery { EventId = id });
        model.Event = result ?? new EventModel();
        return View(model);
    }
}
