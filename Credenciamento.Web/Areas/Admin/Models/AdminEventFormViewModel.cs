using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Web.Areas.Admin.Models;

public class AdminEventFormViewModel : LocalBaseViewModel
{
    public long EventId { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    public string? Description { get; set; }

    [Display(Name = "Local")]
    [MaxLength(255, ErrorMessage = "Local deve ter no máximo 255 caracteres.")]
    public string? Local { get; set; }

    [Display(Name = "Início")]
    [Required(ErrorMessage = "Data de início é obrigatória.")]
    public DateTime Begin { get; set; }

    [Display(Name = "Término")]
    [Required(ErrorMessage = "Data de término é obrigatória.")]
    public DateTime End { get; set; }

    [Display(Name = "Preço")]
    [Required(ErrorMessage = "Preço é obrigatório.")]
    [Range(0, double.MaxValue, ErrorMessage = "Preço não pode ser negativo.")]
    public decimal Price { get; set; }

    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
