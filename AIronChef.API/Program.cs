using AIronChef.API.Extensions;
using AIronChef.Application;
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

            builder.Services.AddHttpClient<OpenAiService>(client =>
            {
                client.BaseAddress = new Uri("https://api.openai.com/v1/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddControllers();
            builder.Services.AddSwaggerDocumentation();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddInfrastructure(connectionString);
            builder.Services.AddApplication();
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
