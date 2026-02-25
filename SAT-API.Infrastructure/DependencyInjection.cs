using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SAT_API.Domain.Entities.Databricks;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces.Parameters;
using SAT_API.Domain.Interfaces.Products;
using SAT_API.Domain.Interfaces.UserOperations;
using SAT_API.Infrastructure.Data;
using SAT_API.Infrastructure.Repositories;
using SAT_API.Infrastructure.Repositories.Databricks;
using SAT_API.Infrastructure.Repositories.Parameters;
using SAT_API.Infrastructure.Repositories.Products;
using SAT_API.Infrastructure.Repositories.UserOperations;

namespace SAT_API.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabricksConnectionSettings>(configuration.GetSection("DatabricksConnectionSettings"));
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
         services.AddHttpClient<DatabricksClient>("DatabricksClient", client =>
        {
            // Configuraciones básicas del cliente
            client.Timeout = TimeSpan.FromMinutes(60);
            // Otros headers por defecto si los necesitas
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
        {
            MaxConnectionsPerServer = 10,
            UseCookies = false,
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
         services.AddLogging();

        // Cargar variables al inicio
        var envService = new EnvironmentService(services.BuildServiceProvider().GetRequiredService<ILogger<EnvironmentService>>());
        if (Path.Exists($"{Directory.GetCurrentDirectory()}/.env"))
        {
            envService.LoadEnvironmentVariables();
        }

        // Registrar el servicio de ambiente
        services.AddSingleton<EnvironmentService>();

        services.AddScoped<IDbContext, DbContext>();
        services.AddScoped<IParametersRepository, ParametersRepository>();
        services.AddScoped<IPistaAuditoriaRepository, PistaAuditoriaRepository>();
        services.AddScoped<IJobConfigRepository, JobConfigRepository>();
        services.AddScoped<IInsumoRepository, InsumoRepository>();
        services.AddScoped<IEjecucionRepository, EjecucionRepository>();
        services.AddScoped<ITrackEjecucionRepository, TrackEjecucionRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        services.AddScoped<IRoleManagementRepository, RoleManagementRepository>();
        services.AddScoped<IDatabricksClient, DatabricksClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var databricksSettings = provider.GetRequiredService<IOptions<DatabricksConnectionSettings>>();
            var httpClient = httpClientFactory.CreateClient();
            return new DatabricksClient(httpClient, databricksSettings.Value.InstanceUrl, databricksSettings.Value.AccessToken, envService);
        });
        services.AddScoped<IDatabricksRepository, DatabricksRepository>();

        return services;
    }
}
