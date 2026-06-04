using Credenciamento.Application.Commands.User;
using Credenciamento.Application.Models;
using Credenciamento.Application.Queries.Person;

namespace Credenciamento.Web.Controllers;

public class ProfileController : LocalControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public ProfileController(
        IMediator mediator,
        IMapper mapper,
        ILogger<ProfileController> logger,
        IServiceProvider services) : base(services)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        var model = _mapper.Map<ProfileIndexViewModel>(GetLocalBaseViewModel());

        if (model.User?.PersonId != null && model.User.PersonId > 0)
        {
            try
            {
                var personQuery = new GetPersonQuery { PersonId = model.User.PersonId };
                var personResult = await _mediator.Send(personQuery);

                if (personResult != null)
                {
                    model.Person = _mapper.Map<PersonModel>(personResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar perfil do usuário {PersonId}", model.User.PersonId);
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAsync(ProfileIndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        try
        {
            // TODO: Implementar comando de atualização de Person
            TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
            return RedirectToAction(nameof(IndexAsync));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar perfil");
            ModelState.AddModelError("", "Erro ao atualizar perfil. Tente novamente.");
            return View("Index", model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ChangePasswordAsync(ProfileIndexViewModel model)
    {
        if (model.Password != model.PasswordConfirm)
            return Problem(statusCode: 400, title: "Error", detail: "Senhas não coincidem");
        try
        {
            var user = GetUserFromToken();
            ChangeUserPasswordCommand command = new()
            {
                Email = user.Email,
                Password = model.Password,
                PasswordConfirm = model.PasswordConfirm
            };

            var result = await _mediator.Send(command);
            if(result.Success)
                TempData["SuccessMessage"] = "Senha alterada com sucesso!";
            else
                TempData["ErrorMessage"] = "Não foi possível alterar a senha!";

            return Ok(true);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: 500, title: "Server error", detail: ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount()
    {
        try
        {
            // TODO: Implementar exclusão de conta
            Response.Cookies.Delete("user-token");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir conta");
            TempData["ErrorMessage"] = "Erro ao excluir conta. Tente novamente.";
            return RedirectToAction(nameof(IndexAsync));
        }
    }
}