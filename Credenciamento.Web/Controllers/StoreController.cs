using Credenciamento.Application.Models;
using Credenciamento.Application.Queries.Event;
using Credenciamento.Web.Models;
using Credenciamento.Web.Services;

namespace Credenciamento.Web.Controllers;

public class StoreController : LocalControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    public StoreController(
        IMapper mapper,
        IMediator mediator,
        IServiceProvider services) : base(services)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(int id)
    {
        var model = _mapper.Map<StoreIndexViewModel>(GetLocalBaseViewModel());
        var result = await _mediator.Send(new GetEventQuery { EventId = id });
        model.Event = result ?? new EventModel();
        model.User = GetUserFromToken();
        return View(model);
    }

    //private UserModel GetUserFromToken()
    //{
    //    if (Request.Cookies.TryGetValue("user-token", out string? token))
    //        return JsonSerializer.Deserialize<UserModel>(StringHelpers.FromBase64(token));

    //    return null;
    //}
}
