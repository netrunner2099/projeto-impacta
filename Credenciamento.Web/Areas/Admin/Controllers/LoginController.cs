using Credenciamento.Application.Queries.User;
using Credenciamento.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;

namespace Credenciamento.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]/[action]")]
public class LoginController : LocalControllerBase
{
    private static string enviromentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    private static readonly CookieOptions authCookieOptions = new CookieOptions
    {
        Expires = DateTimeOffset.UtcNow.AddHours(8),
        HttpOnly = true,
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
        var model = _mapper.Map<AdminLoginViewModel>(GetLocalBaseViewModel());
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Verify(AdminLoginViewModel request)
    {
        if (!ModelState.IsValid)
            return View("Index", request);

        var query = new VerifyLoginQuery
        {
            Email = request.Login,
            Password = request.Password
        };

        var result = await _mediator.Send(query);
        if (result is null)
        {
            _logger.LogWarning("Login admin falhou para o email {Email}", request.Login);
            request.ErrorMessage = "Email ou senha inválidos";
            return View("Index", request);
        }

        Response.Cookies.Append("user-token", result.Token, authCookieOptions);

        return RedirectToAction("Index", "Home", new { area = "Admin" });
    }

    [HttpGet]
    public new IActionResult Logoff()
    {
        Response.Cookies.Delete("user-token");
        return RedirectToAction("Index", "Login", new { area = "Admin" });
    }
}