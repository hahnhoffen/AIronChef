using AIronChef.API.Extensions;
using AIronChef.Application;
using AIronChef.Application.Interfaces;
using AIronChef.Infrastructure;
using AIronChef.Infrastructure.Services;
using System.Net.Http.Headers;

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

            // Adding CORS policy to allow requests from frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod() 
                          .AllowAnyHeader() 
                          .AllowCredentials(); 
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddSwaggerDocumentation();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddInfrastructure(connectionString);
            builder.Services.AddApplication();

            builder.Services.AddHttpClient<IRecipeGenerationService, OpenAiService>();

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
            app.UseCors("AllowFrontend");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
