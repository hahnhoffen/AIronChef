using AIronChef.Application.Interfaces;
using AIronChef.Infrastructure.Database;
using AIronChef.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AIronChef.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<ILoggingService, LoggingService>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString, options =>
                {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                });
            });

            return services;
        }
    }
}
