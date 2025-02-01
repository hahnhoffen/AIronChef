using Microsoft.OpenApi.Models;

namespace AIronChef.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AIronChef API",
                    Version = "v1",
                    Description = "API documentation for the AIronChef application.",
                    Contact = new OpenApiContact
                    {
                        Name = "Team AIronChef",
                        Email = "TBA",
                        //Url = new Uri("TBA")
                        Url = new Uri("https://localhost:7254")
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
