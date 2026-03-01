using Credenciamento.Application.Commands.Ticket;
using Credenciamento.Application.Models;
using Credenciamento.Application.Queries.Event;
using Credenciamento.Application.Queries.Person;
using Credenciamento.Web.Models;
using Credenciamento.Web.Models.Dto;
using Microsoft.Extensions.Configuration;

namespace Credenciamento.Web.Controllers;

[Route("[controller]/[action]")]
public class CheckoutController : Controller
{
    private readonly IMediator _mediator;
    private readonly string baseUrl;
    public CheckoutController(
        IMediator mediator,
        IConfiguration configuration)
    {
        _mediator = mediator;
        baseUrl = $"{configuration["Environment:BaseUrl"]}/ticket/index/";
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
        model.BaseUrl = baseUrl;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicketAsync([FromBody]CheckoutIndexViewModel request)
    {
        var result = await _mediator.Send(new CreateTicketCommand
        {
            EventId = request.EventId ?? 0,
            PersonId = request.PersonId ?? 0,
            Payment = request.Payment ?? 0,
        });

        if (result is null)
            return BadRequest(new { errors = result.ErrorMessages });

        return Ok(new TicketDto { TicketId = result.TicketId });
    }

    [HttpPost]
    public async Task<IActionResult> PayTicketAsync([FromBody]CheckoutIndexViewModel request)
    {
        var result = await _mediator.Send(new PayTicketCommand
        {
            TicketId = request.TicketId ?? 0,
        });

        if (result is null)
            return BadRequest();

        return Ok(new TicketDto { TicketId = result.TicketId, Transaction = result.Transaction });
    }
}
