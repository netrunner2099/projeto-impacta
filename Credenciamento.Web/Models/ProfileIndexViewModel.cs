using Credenciamento.Application.Models;

namespace Credenciamento.Web.Models;

public class ProfileIndexViewModel : LocalBaseViewModel
{
    public PersonModel Person { get; set; }
    public string? Password { get; set; }
    public string? PasswordConfirm { get; set; }
}
