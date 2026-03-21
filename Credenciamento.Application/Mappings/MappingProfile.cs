namespace Credenciamento.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Event, EventModel>().ReverseMap();
        CreateMap<Person, PersonModel>().ReverseMap();


        CreateMap<Ticket, TicketModel>().ReverseMap();
        CreateMap<CreateTicketCommand, Ticket>().ReverseMap();
        CreateMap<CreateTicketCommand, TicketModel>().ReverseMap();
        CreateMap<Ticket, CreateTicketCommandResponse>().ReverseMap();
        CreateMap<TicketModel, CreateTicketCommandResponse>().ReverseMap();
        CreateMap<PayTicketCommand, TicketModel>().ReverseMap();
        CreateMap<PayTicketCommand, Ticket>().ReverseMap();
        CreateMap<TicketModel, PayTicketCommandResponse>().ReverseMap();
        CreateMap<Ticket, PayTicketCommandResponse>().ReverseMap();
        CreateMap<TicketModel, GetTicketQueryResponse>().ReverseMap();
        CreateMap<Ticket, GetTicketQueryResponse>().ReverseMap();

        CreateMap<User, UserModel>().ReverseMap();

        CreateMap<CreatePersonCommand, Person>().ReverseMap();
        CreateMap<CreatePersonCommand, PersonModel>().ReverseMap();
        CreateMap<Person, CreatePersonCommandResponse>().ReverseMap();
        CreateMap<PersonModel, CreatePersonCommandResponse>().ReverseMap();
        CreateMap<Person, GetPersonQueryResponse>().ReverseMap();
        CreateMap<PersonModel, GetPersonQueryResponse>().ReverseMap();

        CreateMap<Event, GetEventQueryResponse>().ReverseMap();
        CreateMap<EventModel, GetEventQueryResponse>().ReverseMap();
    }
}
