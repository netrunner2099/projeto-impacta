using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Web.Models;

public class LoginResetViewModel : LocalBaseViewModel
{
    public string Token { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Informar email")]
    public string Login { get; set; } = string.Empty;

    [Display(Name = "Senha"), DataType(DataType.Password)]
    [Required(ErrorMessage = "Informar senha")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Confirme a Senha"), DataType(DataType.Password)]
    [Required(ErrorMessage = "Confirme a senha")]
    public string PasswordConfirm { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
