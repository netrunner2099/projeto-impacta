using Credenciamento.Application.Commands.Event;
using Credenciamento.Application.Commands.User;
using Credenciamento.Application.Contracts.Ticket;
using Credenciamento.Application.Models;
using Credenciamento.Web.Areas.Admin.Models;
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
        CreateMap<LocalBaseViewModel, LoginResetViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, OnboardIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, StoreIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, TicketIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, ProfileIndexViewModel>().ReverseMap();

        CreateMap<LoginResetViewModel, ResetUserPasswordCommand>().ReverseMap();

        CreateMap<LocalBaseViewModel, AdminHomeIndexViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, AdminLoginViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, AdminUserViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, AdminUserFormViewModel>().ReverseMap();
        CreateMap<AdminUserFormViewModel, CreateUserCommand>().ReverseMap();
        CreateMap<AdminUserFormViewModel, UpdateUserCommand>().ReverseMap();

        CreateMap<LocalBaseViewModel, AdminEventViewModel>().ReverseMap();
        CreateMap<LocalBaseViewModel, AdminEventFormViewModel>().ReverseMap();

        CreateMap<LocalBaseViewModel, AdminTicketViewModel>().ReverseMap();
        CreateMap<AdminEventFormViewModel, CreateEventCommand>().ReverseMap();
        CreateMap<AdminEventFormViewModel, UpdateEventCommand>().ReverseMap();
    }
}
