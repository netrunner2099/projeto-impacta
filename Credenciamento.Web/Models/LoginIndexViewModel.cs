using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Web.Models;

public class LoginIndexViewModel : LocalBaseViewModel
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Informar email")]
    public string Login { get; set; } = string.Empty;

    [Display(Name = "Senha"), DataType(DataType.Password)]
    [Required(ErrorMessage = "Informar senha")]
    public string Password { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
