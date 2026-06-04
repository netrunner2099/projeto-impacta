using Credenciamento.Application.Commands.Event;
using Credenciamento.Application.Queries.Event;
using Credenciamento.Domain.Enums;
using Credenciamento.Web.Areas.Admin.Models;
using Credenciamento.Web.Services;

namespace Credenciamento.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/[controller]/[action]")]
public class EventsController : LocalControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public EventsController(
        ILogger<EventsController> logger,
        IMapper mapper,
        IMediator mediator,
        IServiceProvider services) : base(services)
    {
        _logger = logger;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filterName, byte? filterStatus, bool filterOnlyFuture = true)
    {
        var model = _mapper.Map<AdminEventViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        model.FilterName = filterName;
        model.FilterStatus = filterStatus;
        model.FilterOnlyFuture = filterOnlyFuture;

        var result = await _mediator.Send(new ListEventQuery
        {
            Name = filterName,
            Status = filterStatus,
            OnlyFuture = filterOnlyFuture
        });
        model.Events = result.Events;

        return View(model);
    }

    // ── CREATE ────────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Create()
    {
        var model = _mapper.Map<AdminEventFormViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminEventFormViewModel request)
    {
        var model = _mapper.Map<AdminEventFormViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        if (!ModelState.IsValid)
        {
            _mapper.Map(model, request);
            return View(request);
        }

        var command = _mapper.Map<CreateEventCommand>(request);
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _mapper.Map(model, request);
            request.ErrorMessage = result.Errors.Any()
                ? string.Join(" ", result.Errors)
                : result.Message;
            return View(request);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // ── EDIT ──────────────────────────────────────────────────────────────────

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Edit(long id)
    {
        var model = _mapper.Map<AdminEventFormViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        var result = await _mediator.Send(new GetEventByIdQuery { EventId = id });
        if (result?.Event is null)
            return RedirectToAction(nameof(Index));

        model.EventId = result.Event.EventId;
        model.Name = result.Event.Name;
        model.Description = result.Event.Description;
        model.Local = result.Event.Local;
        model.Begin = result.Event.Begin;
        model.End = result.Event.End;
        model.Price = result.Event.Price;

        return View(model);
    }

    [HttpPost("{id:long}")]
    public async Task<IActionResult> Edit(long id, AdminEventFormViewModel request)
    {
        var model = _mapper.Map<AdminEventFormViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        if (!ModelState.IsValid)
        {
            _mapper.Map(model, request);
            return View(request);
        }

        var command = _mapper.Map<UpdateEventCommand>(request);
        command.EventId = id;
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _mapper.Map(model, request);
            request.ErrorMessage = result.Errors.Any()
                ? string.Join(" ", result.Errors)
                : result.Message;
            return View(request);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // ── ACTIVATE ─────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Activate([FromForm] long id)
    {
        var base_ = GetLocalBaseViewModel();
        if (base_?.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        await _mediator.Send(new ActivateEventCommand { EventId = id });
        return RedirectToAction(nameof(Index));
    }

    // ── INACTIVATE ────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Inactivate([FromForm] long id)
    {
        var base_ = GetLocalBaseViewModel();
        if (base_?.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        await _mediator.Send(new InactivateEventCommand { EventId = id });
        return RedirectToAction(nameof(Index));
    }

    // ── DELETE ────────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Delete([FromForm] long id)
    {
        var base_ = GetLocalBaseViewModel();
        if (base_?.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        await _mediator.Send(new DeleteEventCommand { EventId = id });
        return RedirectToAction(nameof(Index));
    }
}
