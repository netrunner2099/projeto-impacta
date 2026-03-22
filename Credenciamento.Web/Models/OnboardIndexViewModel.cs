using Credenciamento.Application.Models;

namespace Credenciamento.Web.Models;

public class OnboardIndexViewModel : LocalBaseViewModel
{
    public EventModel Event { get; set; }
    public PersonModel Person { get; set; }

    public IEnumerable<string> Errors { get; set; } 
}

