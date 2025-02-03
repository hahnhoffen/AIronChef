using AIronChef.Application.Interfaces;
using AIronChef.Domain.Common;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using AIronChef.Infrastructure.Database;
using AIronChef.Infrastructure.Logging;
using AIronChef.Infrastructure.Repositories;
using AIronChef.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AIronChef.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IGenericRepository<Recipe>, GenericRepository<Recipe>>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();

            services.AddScoped<IRecipeGenerationService, OpenAiService>();

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
