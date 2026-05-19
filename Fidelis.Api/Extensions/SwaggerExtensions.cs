using System.IO;
using System.Reflection;
using Microsoft.OpenApi;

namespace Fidelis.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Fidelis API",
                Version = "v1",
                Description = "API REST para gerenciamento do dominio Fidelis.",
                TermsOfService = new Uri("https://example.com/fidelis/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Equipe Fidelis",
                    Email = "contato@example.com",
                    Url = new Uri("https://example.com/fidelis")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://example.com/fidelis/license")
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });
    }
}