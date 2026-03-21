using Credenciamento.Application.Interfaces.Global;
using Credenciamento.Application.Services;

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

        InfrastructureRegistration.AddInfrastructure(services, configuration);

        return services;
    }
}


