using System.ComponentModel.DataAnnotations;

namespace Credenciamento.Application.Models;

public class PersonModel
{
    public long PersonId { get; set; }

    [Display(Name = "Nome Completo")]
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(255, ErrorMessage = "O nome deve ter no máximo 255 caracteres")]
    public string Name { get; set; }

    [Display(Name = "CPF")]
    [Required(ErrorMessage = "O CPF é obrigatório")]
    [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido")]
    public string Document { get; set; }

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [StringLength(255)]
    public string Email { get; set; }

    [Display(Name = "Celular")]
    [Required(ErrorMessage = "O telefone é obrigatório")]
    [StringLength(20)]
    [RegularExpression(@"^\(\d{2}\)\s\d{5}-\d{4}$", ErrorMessage = "Telefone inválido")]
    public string Phone { get; set; }

    [Display(Name = "Nascimento")]
    [Required(ErrorMessage = "O nascimento é obrigatório")]
    public DateTime? BirthDay { get; set; }

    [Display(Name = "CEP")]
    [Required(ErrorMessage = "O CEP é obrigatório")]
    [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "CEP inválido")]
    public string ZipCode { get; set; }

    [Display(Name = "Endereço")]
    [Required(ErrorMessage = "O endereço é obrigatório")]
    [StringLength(255)]
    public string Address { get; set; }

    [Display(Name = "Número")]
    [Required(ErrorMessage = "O número é obrigatório")]
    [StringLength(50)]
    public string Number { get; set; }

    [Display(Name = "Complemento")]
    public string Complement { get; set; }

    [Display(Name = "Bairro")]
    [Required(ErrorMessage = "O bairro é obrigatório")]
    [StringLength(100)]
    public string Neighborhood { get; set; }
    
    [Display(Name = "Cidade")]
    [Required(ErrorMessage = "A cidade é obrigatória")]
    [StringLength(100)]
    public string City { get; set; }

    [Display(Name = "Estado")]
    [Required(ErrorMessage = "O estado é obrigatório")]
    public string State { get; set; }

    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public IEnumerable<TicketModel> Tickets { get; set; }
}