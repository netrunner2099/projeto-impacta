using Credenciamento.Application.Queries.Event;

namespace Credenciamento.Web.Controllers;

public class HomeController : LocalControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public HomeController(
        ILogger<HomeController> logger,
        IMapper mapper,
        IMediator mediator,
        IServiceProvider services) : base(services)
    {
        _logger = logger;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = _mapper.Map<HomeIndexViewModel>(GetLocalBaseViewModel());
        try
        {
            var result = await _mediator.Send(new ListFutureEventQuery());
            model.Events = result.Events.OrderBy(o => o.Begin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Index: {0}", ex.Message);
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> LogOff()
    {
        var logoff = base.Logoff();
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        var model = _mapper.Map<HomeIndexViewModel>(GetLocalBaseViewModel());
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
