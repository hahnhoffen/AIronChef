
using System.Data.SqlClient;
using AIronChef.API.Extensions;
using AIronChef.Infrastructure.Logging;
using AIronChef.Infrastructure;

namespace AIronChef.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string connectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING")!;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Azure SQL connection string is not set in environment variables.");
            }



            builder.Services.AddControllers();
            builder.Services.AddSwaggerDocumentation();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSingleton<LoggingService>();

            builder.Services.AddInfrastructure(connectionString);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AIronChef API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
