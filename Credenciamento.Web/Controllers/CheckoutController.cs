using Credenciamento.Application.Models;
using Credenciamento.Application.Queries.Event;
using Credenciamento.Application.Queries.Person;

using Credenciamento.Web.Models;

namespace Credenciamento.Web.Controllers;

public class CheckoutController : Controller
{
    private readonly IMediator _mediator;
    public CheckoutController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}/{personId}")]
    public async Task<IActionResult> Index(int id, int personId)
    {
        var model = new CheckoutIndexViewModel();
        var result = await _mediator.Send(new GetEventQuery { EventId = id });
        model.Event = result ?? new EventModel();
        model.Person = new PersonModel();

        var personResult = await _mediator.Send(new GetPersonQuery { PersonId = personId });
        model.Person = personResult ?? new PersonModel();
        model.Person.Document = model.Person.Document.MaskCpf();

        return View(model);
    }
}
