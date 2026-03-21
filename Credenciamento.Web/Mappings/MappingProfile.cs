using Credenciamento.Application.Contracts.Ticket;
using Credenciamento.Application.Models;
using Credenciamento.Web.Models;
using Credenciamento.Web.Models.Dto;

namespace Credenciamento.Web.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EventModel, EventDto>().ReverseMap();
        CreateMap<PersonModel, PersonDto>().ReverseMap();

        CreateMap<GetTicketQueryResponse, TicketDto>().ReverseMap();

        CreateMap<LocalBaseViewModel, CheckoutIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, ErrorViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, HomeIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, LoginIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, OnboardIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, StoreIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, TicketIndexViewModel>().ReverseMap();
    }
}
