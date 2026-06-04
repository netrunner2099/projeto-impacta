using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Web.Areas.Admin.Models;

public class AdminUserFormViewModel : LocalBaseViewModel
{
    public long UserId { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(150, ErrorMessage = "E-mail deve ter no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Senha")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Confirmar Senha")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "As senhas não coincidem.")]
    public string? PasswordConfirm { get; set; }

    [Display(Name = "Perfil")]
    [Required(ErrorMessage = "Perfil é obrigatório.")]
    [Range(1, byte.MaxValue, ErrorMessage = "Perfil é obrigatório.")]
    public byte Role { get; set; }

    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

    // --- Dados de Person (apenas para role = User) ---
    public long PersonId { get; set; }

    [Display(Name = "CPF")]
    [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido.")]
    public string? PersonDocument { get; set; }

    [Display(Name = "Celular")]
    [RegularExpression(@"^\(\d{2}\)\s\d{5}-\d{4}$", ErrorMessage = "Telefone inválido.")]
    public string? PersonPhone { get; set; }

    [Display(Name = "Nascimento")]
    public DateTime? PersonBirthDay { get; set; }

    [Display(Name = "CEP")]
    [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "CEP inválido.")]
    public string? PersonZipCode { get; set; }

    [Display(Name = "Endereço")]
    public string? PersonAddress { get; set; }

    [Display(Name = "Número")]
    public string? PersonNumber { get; set; }

    [Display(Name = "Complemento")]
    public string? PersonComplement { get; set; }

    [Display(Name = "Bairro")]
    public string? PersonNeighborhood { get; set; }

    [Display(Name = "Cidade")]
    public string? PersonCity { get; set; }

    [Display(Name = "Estado")]
    public string? PersonState { get; set; }
}
