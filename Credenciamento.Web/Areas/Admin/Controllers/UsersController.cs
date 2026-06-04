using Credenciamento.Application.Commands.Person;
using Credenciamento.Application.Commands.User;
using Credenciamento.Application.Queries.User;
using Credenciamento.Domain.Enums;
using Credenciamento.Domain.Interfaces;
using Credenciamento.Web.Areas.Admin.Models;
using Credenciamento.Web.Services;

namespace Credenciamento.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/[controller]/[action]")]
public class UsersController : LocalControllerBase
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IPersonRepository _personRepository;

    public UsersController(
        ILogger<UsersController> logger,
        IMapper mapper,
        IMediator mediator,
        IPersonRepository personRepository,
        IServiceProvider services) : base(services)
    {
        _logger = logger;
        _mapper = mapper;
        _mediator = mediator;
        _personRepository = personRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filterName, byte? filterRole)
    {
        var model = _mapper.Map<AdminUserViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        model.FilterName = filterName;
        model.FilterRole = filterRole;

        var result = await _mediator.Send(new ListUserQuery { Name = filterName, Role = filterRole });
        model.Users = result.Users;

        return View(model);
    }

    // ── CREATE ────────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Create()
    {
        var model = _mapper.Map<AdminUserFormViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminUserFormViewModel request)
    {
        var model = _mapper.Map<AdminUserFormViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        if (!ModelState.IsValid)
        {
            _mapper.Map(model, request);
            return View(request);
        }

        var command = _mapper.Map<CreateUserCommand>(request);
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _mapper.Map(model, request);
            request.ErrorMessage = result.Errors.Any()
                ? string.Join(" ", result.Errors)
                : result.Message;
            return View(request);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // ── EDIT ──────────────────────────────────────────────────────────────────

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Edit(long id)
    {
        var model = _mapper.Map<AdminUserFormViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        var result = await _mediator.Send(new GetUserByIdQuery { UserId = id });
        if (result?.User is null)
            return RedirectToAction(nameof(Index));

        model.UserId = result.User.UserId;
        model.Name = result.User.Name;
        model.Email = result.User.Email;
        model.Role = result.User.Role;

        if (result.User.Role == (byte)UserRole.User)
        {
            var person = await _personRepository.GetByEmailAsync(result.User.Email);
            if (person is not null)
            {
                model.PersonId = person.PersonId;
                model.PersonDocument = person.Document;
                model.PersonPhone = person.Phone;
                model.PersonBirthDay = person.BirthDay;
                model.PersonZipCode = person.ZipCode;
                model.PersonAddress = person.Address;
                model.PersonNumber = person.Number;
                model.PersonComplement = person.Complement;
                model.PersonNeighborhood = person.Neighborhood;
                model.PersonCity = person.City;
                model.PersonState = person.State;
            }
        }

        return View(model);
    }

    [HttpPost("{id:long}")]
    public async Task<IActionResult> Edit(long id, AdminUserFormViewModel request)
    {
        var model = _mapper.Map<AdminUserFormViewModel>(GetLocalBaseViewModel());
        if (model is null || model.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        // Ignora validação de senha no Edit
        ModelState.Remove(nameof(AdminUserFormViewModel.Password));
        ModelState.Remove(nameof(AdminUserFormViewModel.PasswordConfirm));

        if (!ModelState.IsValid)
        {
            _mapper.Map(model, request);
            return View(request);
        }

        var command = _mapper.Map<UpdateUserCommand>(request);
        command.UserId = id;
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            _mapper.Map(model, request);
            request.ErrorMessage = result.Errors.Any()
                ? string.Join(" ", result.Errors)
                : result.Message;
            return View(request);
        }

        if (request.Role == (byte)UserRole.User && request.PersonId > 0)
        {
            var personCommand = new UpdatePersonCommand
            {
                PersonId = request.PersonId,
                Name = request.Name,
                Document = request.PersonDocument ?? string.Empty,
                Email = request.Email,
                Phone = request.PersonPhone ?? string.Empty,
                BirthDay = request.PersonBirthDay,
                ZipCode = request.PersonZipCode ?? string.Empty,
                Address = request.PersonAddress ?? string.Empty,
                Number = request.PersonNumber ?? string.Empty,
                Complement = request.PersonComplement ?? string.Empty,
                Neighborhood = request.PersonNeighborhood ?? string.Empty,
                City = request.PersonCity ?? string.Empty,
                State = request.PersonState ?? string.Empty,
            };
            await _mediator.Send(personCommand);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // ── ACTIVATE ─────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Activate([FromForm] long id)
    {
        var base_ = GetLocalBaseViewModel();
        if (base_?.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        await _mediator.Send(new ActivateUserCommand { UserId = id });
        return RedirectToAction(nameof(Index));
    }

    // ── INACTIVATE ────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Inactivate([FromForm] long id)
    {
        var base_ = GetLocalBaseViewModel();
        if (base_?.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        await _mediator.Send(new InactivateUserCommand { UserId = id });
        return RedirectToAction(nameof(Index));
    }

    // ── DELETE ────────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Delete([FromForm] long id)
    {
        var base_ = GetLocalBaseViewModel();
        if (base_?.User?.Role != (byte)UserRole.Admin)
            return RedirectToAction("Index", "Login", new { area = "Admin" });

        await _mediator.Send(new DeleteUserCommand { UserId = id });
        return RedirectToAction(nameof(Index));
    }
}

