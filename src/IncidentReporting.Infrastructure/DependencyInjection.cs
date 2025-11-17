using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Infrastructure.Data;
using IncidentReporting.Infrastructure.Repositories;

namespace IncidentReporting.Infrastructure
{
    /// <summary>
    /// Registers all Infrastructure services into the DI container.
    /// This is called from Program.cs inside the API project.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register SQLite DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                  ?? "Data Source=Data/incidents.db";

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            // Repository registrations
            services.AddScoped<IIncidentRepository, IncidentRepository>();
            services.AddScoped<IIncidentHistoryRepository, IncidentHistoryRepository>();

            return services;
        }
    }
}
