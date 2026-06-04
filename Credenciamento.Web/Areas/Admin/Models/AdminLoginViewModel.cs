using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Web.Areas.Admin.Models;

public class AdminLoginViewModel : LocalBaseViewModel
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Informar email")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Login { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    [Required(ErrorMessage = "Informar senha")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
