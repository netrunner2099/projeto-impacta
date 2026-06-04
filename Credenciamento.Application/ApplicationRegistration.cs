using Credenciamento.Application.Commands.Event;
using Credenciamento.Application.Commands.User;
using Credenciamento.Application.Interfaces.Global;
using Credenciamento.Application.Services;
using Credenciamento.Application.Validators.Event;
using Credenciamento.Application.Validators.User;

namespace Credenciamento.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, 
        Profile? profile, 
        IConfiguration configuration)
    {
        // Adding Mediator
        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = $"{configuration["LuckyPenny:LicenseKey"]}"; 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        #region "AutoMapper"
        // Forma moderna - registra automaticamente todos os profiles
        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = $"{configuration["LuckyPenny:LicenseKey"]}";
            cfg.AddProfile<MappingProfile>();
            if (profile is not null)
            {
                cfg.AddProfile(profile);
            }
        }, Assembly.GetExecutingAssembly());
        #endregion

        // Services
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IQrCodeClient, QrCodeClient>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IUserService, UserService>();

        // Validators
        services.AddScoped<IValidator<CreatePersonCommand>, CreatePersonCommandValidator>();
        services.AddScoped<IValidator<CreateTicketCommand>, CreateTicketCommandValidator>();
        services.AddScoped<IValidator<ChangeUserPasswordCommand>, ChangeUserPasswordCommandValidator>();
        services.AddScoped<IValidator<ResetUserPasswordCommand>, ResetUserPasswordCommandValidator>();
        services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserCommandValidator>();
        services.AddScoped<IValidator<CreateEventCommand>, CreateEventCommandValidator>();
        services.AddScoped<IValidator<UpdateEventCommand>, UpdateEventCommandValidator>();

        InfrastructureRegistration.AddInfrastructure(services, configuration);

        return services;
    }
}


