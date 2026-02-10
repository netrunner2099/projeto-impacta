namespace Credenciamento.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Event, EventModel>().ReverseMap();
        CreateMap<Person, PersonModel>().ReverseMap();
        CreateMap<Ticket, TicketModel>().ReverseMap();
        CreateMap<User, UserModel>().ReverseMap();

        CreateMap<CreatePersonCommand, Person>().ReverseMap();
        CreateMap<CreatePersonCommand, PersonModel>().ReverseMap();
        CreateMap<Person, CreatePersonCommandResponse>().ReverseMap();
        CreateMap<PersonModel, CreatePersonCommandResponse>().ReverseMap();

        CreateMap<Event, GetEventQueryResponse>().ReverseMap();
        CreateMap<EventModel, GetEventQueryResponse>().ReverseMap();
    }
}



