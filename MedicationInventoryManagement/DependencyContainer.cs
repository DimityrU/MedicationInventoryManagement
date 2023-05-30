using MedicationInventoryManagement.Services;
using MedicationInventoryManagement.Services.Interfaces;

namespace MedicationInventoryManagement;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyContainer
{
    public static IServiceCollection Configure(this IServiceCollection services)
    {
        // Registered services
        services.AddScoped<ILogInService, LogInService>();
        services.AddScoped<IMedicationService, MedicationService>();

        return services;
    }
}