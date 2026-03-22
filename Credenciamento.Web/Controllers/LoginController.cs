using Credenciamento.Application.Queries.User;
using Credenciamento.Shared.Helpers;
using Credenciamento.Web.Models;
using Credenciamento.Web.Services;
using Microsoft.AspNetCore.Http;

namespace Credenciamento.Web.Controllers;

[Route("[controller]/[action]")]
public class LoginController : LocalControllerBase
{
    private static string enviromentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    private static readonly CookieOptions authCookieOptions = new CookieOptions
    {
        Expires = DateTimeOffset.UtcNow.AddHours(8), // 8 horas
        HttpOnly = true, // Não acessível via JavaScript (mais seguro)
        Secure = enviromentName == "production",
        SameSite = enviromentName == "production" ? SameSiteMode.Strict : SameSiteMode.Lax,
        Path = "/"
    };
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public LoginController(
        ILogger<LoginController> logger,
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
            _logger.LogWarning("Login falhou para o email {Email}", request.Login);
            request.ErrorMessage = "Email ou senha inválidos";
            return View("Index", request);
        }

        // Gravando o cookie de autenticação
        Response.Cookies.Append("user-token", result.Token, authCookieOptions);

        if (Request.Cookies.TryGetValue("store-eventId", out string? eventId))
            return RedirectToAction("Index","Checkout", new { id = eventId, personId = result.PersonId });

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("{login}")]
    public IActionResult Forgot(string login)
    {
        var model = new LoginIndexViewModel();
        model.Login = !string.IsNullOrEmpty(login) ? StringHelpers.FromBase64(login) : "";
        model.SuccessMessage = "Foi enviado um email com a nova senha para você.<br/>Caso não encontre, verifique a sua caixa de Spam, por favor.";

        return View("Index", model);
    }

    [HttpGet("{login}")]
    public async Task<IActionResult> OneTime(string login)
    {
        var model = new LoginIndexViewModel();
        await _mediator.Send(new GetOtpCodeQuery { Email = StringHelpers.FromBase64(login) });
        model.SuccessMessage = "Código OTP gerado com sucesso. Verifique seu email.";

        return View("Index", model);
    }
}
