using Credenciamento.Application.Models;
using Credenciamento.Web.Models;

namespace Credenciamento.Web.Areas.Admin.Models;

public class AdminTicketViewModel : LocalBaseViewModel
{
    // Filters
    public long? FilterEventId { get; set; }
    public byte? FilterStatus { get; set; }
    public byte? FilterPayment { get; set; }

    // Data
    public IEnumerable<TicketModel> Tickets { get; set; } = [];
    public IEnumerable<EventModel> Events { get; set; } = [];
}
