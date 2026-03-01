using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Web.Models;

public class LoginIndexViewModel
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Informar email")]
    public string Login { get; set; }

    [Display(Name = "Senha"), DataType(DataType.Password)]
    [Required(ErrorMessage = "Informar senha")]
    public string Password { get; set; }

    public string SuccessMessage { get; set; }
    public string ErrorMessage { get; set; }
}
