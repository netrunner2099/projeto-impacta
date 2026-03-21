using Credenciamento.Application.Models;
using Credenciamento.Application.Queries.Event;
using Credenciamento.Shared.Helpers;
using Credenciamento.Web.Models;
using System.Text.Json;

namespace Credenciamento.Web.Controllers;

public class StoreController : Controller
{
    private readonly IMediator _mediator;
    public StoreController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(int id)
    {
        var model = new StoreIndexViewModel();
        var result = await _mediator.Send(new GetEventQuery { EventId = id });
        model.Event = result ?? new EventModel();
        model.User = GetUserFromToken();
        return View(model);
    }

    private UserModel GetUserFromToken()
    {
        if (Request.Cookies.TryGetValue("user-token", out string? token))
            return JsonSerializer.Deserialize<UserModel>(StringHelpers.FromBase64(token));

        return null;
    }
}
