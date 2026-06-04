using Credenciamento.Application.Contracts.Ticket;
using Credenciamento.Application.Queries.Ticket;
using Credenciamento.Domain.Enums;
using Credenciamento.Domain.Interfaces;
using Credenciamento.Web.Areas.Admin.Models;
using Credenciamento.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Credenciamento.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/[controller]/[action]")]
    public class TicketsController : LocalControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;

        public TicketsController(
            ILogger<TicketsController> logger,
            IMediator mediator,
            IMapper mapper,
            IEventRepository eventRepository,
            IServiceProvider services) : base(services)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _eventRepository = eventRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long? filterEventId, byte? filterStatus, byte? filterPayment)
        {
            var vm = _mapper.Map<AdminTicketViewModel>(GetLocalBaseViewModel());
            if (vm is null || vm.User?.Role != (byte)UserRole.Admin)
                return RedirectToAction("Index", "Login", new { area = "Admin" });

            var result = await _mediator.Send(new ListTicketAdminQuery
            {
                EventId = filterEventId,
                Status = filterStatus,
                Payment = filterPayment
            });

            var events = await _eventRepository.ListAllAsync();

            vm.FilterEventId = filterEventId;
            vm.FilterStatus = filterStatus;
            vm.FilterPayment = filterPayment;
            vm.Tickets = result.Tickets;
            vm.Events = _mapper.Map<IEnumerable<Credenciamento.Application.Models.EventModel>>(events);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Voucher(long id)
        {
            var result = await _mediator.Send(new GetTicketQuery { TicketId = id });

            if (result is null || !result.Success)
            {
                TempData["Error"] = result?.Message ?? "Ticket não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(result);
        }
    }
}
