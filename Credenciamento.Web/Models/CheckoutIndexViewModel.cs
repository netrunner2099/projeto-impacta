using Credenciamento.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Web.Models;

public class CheckoutIndexViewModel
{
    public string BaseUrl { get; set; }
    public long? TicketId { get; set; }
    public long? EventId { get; set; }
    public EventModel? Event { get; set; }

    public byte? Payment { get; set; }

    public long? PersonId { get; set; }
    public PersonModel? Person { get; set; }

    public IEnumerable<string>? Errors { get; set; }

    [Display(Name = "Número do cartão")]
    [Required(ErrorMessage = "O número é obrigatório")]
    public string? CardNumber { get; set; }

    [Display(Name = "Expiração")]
    [Required(ErrorMessage = "A expiração é obrigatória")]
    public string? CardExpiration { get; set; }

    [Display(Name = "Cvv")]
    [Required(ErrorMessage = "O cvv é obrigatório")]
    public string? CardCVV { get; set; }
}

