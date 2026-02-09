namespace Credenciamento.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Event, EventModel>().ReverseMap();
        CreateMap<Person, PersonModel>().ReverseMap();
        CreateMap<Ticket, TicketModel>().ReverseMap();
        CreateMap<User, UserModel>().ReverseMap();
    }
}


