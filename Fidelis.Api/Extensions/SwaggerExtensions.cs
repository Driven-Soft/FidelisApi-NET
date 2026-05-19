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
                Description = "API REST para gerenciamento do domínio Fidelis — plataforma de saúde contínua para pets. Centraliza a jornada clínica do animal, conectando tutores e veterinários em torno de um histórico longitudinal estruturado. Expõe recursos para gestão de pets, tutores, veterinários, clínicas, consultas, vacinas, prescrições, exames, lembretes e recomendações inteligentes.",
                Contact = new OpenApiContact
                {
                    Name = "Equipe Driven Soft",
                    Email = "contato@drivensoft.com",
                    Url = new Uri("https://github.com/Driven-Soft")
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