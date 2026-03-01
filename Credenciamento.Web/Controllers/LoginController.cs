using Credenciamento.Application.Queries.User;
using Credenciamento.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Credenciamento.Web.Controllers;

[Route("[controller]/[action]")]
public class LoginController : Controller
{
    private static readonly CookieOptions authCookieOptions = new CookieOptions
    {
        Expires = DateTimeOffset.UtcNow.AddHours(8), // 8 horas
        HttpOnly = true, // Não acessível via JavaScript (mais seguro)
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/"
    };

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
    public async Task<IActionResult> Verify(LoginIndexViewModel request)
    {
        var query = new VerifyLoginQuery
        {
            Email = request.Login,
            Password = request.Password
        }; 

        var result = await _mediator.Send(query);
        if(result is null)
        {
            request.ErrorMessage = "Email ou senha inválidos";
            return View("Index", request);
        }

        // Gravando o cookie de autenticação
        Response.Cookies.Append("user-token", result.Token.ToString(), authCookieOptions);
        if (Request.Cookies.TryGetValue("store-eventId", out string? eventId))
            return RedirectToAction("Index","Checkout", new { id = eventId, personId = result.PersonId });

        return View("Index", request);
    }

    [HttpGet("{login}")]
    public IActionResult Forgot(string login)
    {
        var model = new LoginIndexViewModel();
        model.Login = "";
        model.SuccessMessage = "Foi enviado um email com a nova senha para você.<br/>Caso não encontre, verifique a sua caixa de Spam, por favor.";

        return View("Index", model);
    }
}
