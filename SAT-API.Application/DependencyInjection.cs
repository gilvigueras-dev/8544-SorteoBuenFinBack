using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Interfaces.Databricks;
using SAT_API.Application.Interfaces.Parameters;
using SAT_API.Application.Interfaces.Products;
using SAT_API.Application.Middlewares;
using SAT_API.Application.Middlewares.Validators;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using SAT_API.Application.Services;
using SAT_API.Application.Services.Databricks;
using SAT_API.Application.Services.Parameters;
using SAT_API.Application.Services.Products;
using System.Reflection;
using SAT_API.Application.Interfaces.UserOperations;
using SAT_API.Application.Services.UserOperations;
using SAT_API.Application.Interfaces.Excel;
using SAT_API.Application.Services.Excel;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace SAT_API.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IDatabricksService, DatabricksService>();
        services.AddScoped<IProductsInfrastructure, ProductsInfrastructure>();
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IParametersService, ParametersService>();
        services.AddScoped<IPistaAuditoriaInfrastructure, PistaAuditoriaInfrastructure>();
        services.AddScoped<IPistaAuditoriaServices, PistaAuditoriaServices>();
        services.AddScoped<ITrackEjecucionInfrastructure, TrackEjecucionInfrastructure>();
        services.AddScoped<ITrackEjecucionServices, TrackEjecucionServices>();
        services.AddScoped<IEjecucionInfrastructure, EjecucionInfrastructure>();
        services.AddScoped<IEjecucionService, EjecucionService>();
        services.AddScoped<IJobConfigService, JobConfigService>();
        services.AddScoped<IInsumoInfrastructure, InsumoInfrastructure>();
        services.AddScoped<IInsumoService, InsumoService>();
        services.AddScoped<IValidationService, ValidationService>();

        services.AddScoped<IClaimService, ClaimsService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRoleManagementService, RoleManagementService>();
        services.AddScoped<IAddressClientService, AddressClientService>();
        services.AddScoped<IExcelService, ExcelService>();

        services.AddSingleton<ITranslator, JsonTranslator>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.Configure<IISServerOptions>(options =>  options.MaxRequestBodySize = 2_147_483_648/* 2GB*/);
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 2_147_483_648; // 2GB
            options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(60);
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(60);
        });

        services
        .AddAuthentication()
        .AddBearerToken(options =>
        {
            options.Events = TokenConfig.ConfigureBearerTokenEvents();
        });

        return services;
    }

    /// <summary>
    /// Configura los middlewares de la aplicación
    /// </summary>
    public static IApplicationBuilder AddMiddleware(this IApplicationBuilder app)
    {
        app.ExceptionEnvironmentMiddleware();
        app.ExceptionMiddleware();
        app.ValidationMiddleware();
        app.CultureMiddleware();
        return app;
    }
}