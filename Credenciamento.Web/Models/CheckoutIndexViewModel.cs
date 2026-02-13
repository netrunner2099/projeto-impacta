using Credenciamento.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Web.Models;

public class CheckoutIndexViewModel
{
    public EventModel Event { get; set; }
    public PersonModel Person { get; set; }
    public IEnumerable<string> Errors { get; set; }

    [Display(Name = "Número do cartão")]
    [Required(ErrorMessage = "O número é obrigatório")]
    public string CardNumber { get; set; }

    [Display(Name = "Expiração")]
    [Required(ErrorMessage = "A expiração é obrigatória")]
    public string CardExpiration { get; set; }

    [Display(Name = "Cvv")]
    [Required(ErrorMessage = "O cvv é obrigatório")]
    public string CardCVV { get; set; }
}

