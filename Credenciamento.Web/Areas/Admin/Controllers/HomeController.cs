using Credenciamento.Domain.Enums;

namespace Credenciamento.Web.Areas.Admin.Controllers;

[Area(nameof(Admin))]
[Route("admin/[controller]/[action]")]

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
    public IActionResult Index()
    {
        var model = _mapper.Map<HomeIndexViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Home", new { area = "" });

        return View(model);
    }
}
