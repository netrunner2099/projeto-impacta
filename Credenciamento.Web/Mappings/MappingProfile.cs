using Credenciamento.Application.Contracts.Ticket;
using Credenciamento.Application.Models;
using Credenciamento.Web.Models.Dto;

namespace Credenciamento.Web.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EventModel, EventDto>().ReverseMap();
        CreateMap<PersonModel, PersonDto>().ReverseMap();

        CreateMap<GetTicketQueryResponse, TicketDto>().ReverseMap();
            //.ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => (long?)src.TicketId))
            //.ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => (long?)src.PersonId))
            //.ForMember(dest => dest.EventId, opt => opt.MapFrom(src => (long?)src.EventId))
            //.ForMember(dest => dest.Price, opt => opt.MapFrom(src => (decimal?)src.Price))
            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte?)src.Status))
            //.ForMember(dest => dest.Transaction, opt => opt.MapFrom(src => src.Transaction))
            //.ForMember(dest => dest.Auth, opt => opt.MapFrom(src => src.Auth))
            //.ForMember(dest => dest.QRCodeResponse, opt => opt.MapFrom(src => src.QRCodeResponse))
            //.ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event))
            //.ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person));
    }
}
