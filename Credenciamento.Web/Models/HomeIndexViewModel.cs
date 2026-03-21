using Credenciamento.Application.Models;
using System.Collections.Generic;

namespace Credenciamento.Web.Models;

public class HomeIndexViewModel
{
    public IEnumerable<EventModel> Events { get; set; }
}
