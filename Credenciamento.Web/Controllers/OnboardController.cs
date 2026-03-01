using AutoMapper;
using Credenciamento.Application.Commands.Person;
using Credenciamento.Application.Models;
using Credenciamento.Application.Queries.Event;
using Credenciamento.Web.Models;

namespace Credenciamento.Web.Controllers;

public class OnboardController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public OnboardController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int id)
    {
        var model = new OnboardIndexViewModel();
        var result = await _mediator.Send(new GetEventQuery { EventId = id });
        model.Event = result ?? new EventModel();
        model.Person = new PersonModel();
        model.Person.BirthDay = null;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(OnboardIndexViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var result = await _mediator.Send(new GetEventQuery { EventId = model.Event.EventId });
        if (result == null)
            return View("Index", model);

        var command = _mapper.Map<CreatePersonCommand>(model.Person);
        var commandResult = await _mediator.Send(command);
        model.Errors = commandResult.Errors;
        if(model.Errors is not null && model.Errors.Any())
            return View("Index", model);

        return RedirectToAction("Index", "Checkout", new { id = model.Event.EventId, personId = commandResult.PersonId });
    }
}