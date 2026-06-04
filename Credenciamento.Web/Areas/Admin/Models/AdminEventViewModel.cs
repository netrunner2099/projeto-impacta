using Credenciamento.Application.Models;

namespace Credenciamento.Web.Areas.Admin.Models;

public class AdminEventViewModel : LocalBaseViewModel
{
    public string? FilterName { get; set; }
    public byte? FilterStatus { get; set; }
    public bool FilterOnlyFuture { get; set; } = true;
    public IEnumerable<EventModel> Events { get; set; } = [];
}
