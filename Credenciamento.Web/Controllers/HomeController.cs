using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Credenciamento.Application.Queries.Event;
using Credenciamento.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Credenciamento.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public HomeController(
        ILogger<HomeController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeIndexViewModel();
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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
