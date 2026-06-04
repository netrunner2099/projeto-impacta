using Credenciamento.Application.Models;
using System.Collections.Generic;

namespace Credenciamento.Web.Models;

public class HomeIndexViewModel : LocalBaseViewModel
{
    public IEnumerable<EventModel> Events { get; set; }
}
