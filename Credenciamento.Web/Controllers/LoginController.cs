using Credenciamento.Web.Models;

namespace Credenciamento.Web.Controllers;

[Route("[controller]/[action]")]
public class LoginController : Controller
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public LoginController(
        ILogger<LoginController> logger, 
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new LoginIndexViewModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult Verify(LoginIndexViewModel request)
    {
        request.ErrorMessage = "Erro";
        return View("Index", request);
    }

    [HttpGet("{login}")]
    public IActionResult Forgot(string login)
    {
        var model = new LoginIndexViewModel();
        model.Login = "";
        model.SuccessMessage = "Foi enviado um email com a nova senha para você.";

        return View("Index", model);
    }
}
