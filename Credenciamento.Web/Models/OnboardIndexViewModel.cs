using Credenciamento.Application.Models;
using System.Collections.Generic;

namespace Credenciamento.Web.Models;

public class OnboardIndexViewModel
{
    public EventModel Event { get; set; }
    public PersonModel Person { get; set; }

    public IEnumerable<string> Errors { get; set; } 
}

