using Credenciamento.Application.Commands.User;
using Credenciamento.Application.Mappings;
using Credenciamento.Application.Validators.Person;
using Credenciamento.Application.Validators.User;
using Credenciamento.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Credenciamento.Application;


public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Profile? profile, IConfiguration configuration)
    {
        // Adding Mediator
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });      

        #region "AutoMapper"
        var _mapperconf = new MapperConfiguration(mc => {
            mc.AddProfile(new MappingProfile());
            if (profile is not null)
            {
                mc.AddProfile(profile);
            }
        });
        IMapper _mapper = _mapperconf.CreateMapper();
        services.AddSingleton(_mapper);
        #endregion

        // Validators
        services.AddScoped<IValidator<CreatePersonCommand>, CreatePersonCommandValidator>();
        services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();

        InfrastructureRegistration.AddInfrastructure(services, configuration);

        return services;
    }
}


