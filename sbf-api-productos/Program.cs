using Microsoft.OpenApi.Models;
using SAT_API.Application;
using SAT_API.Application.Middlewares;
using SAT_API.Domain.Entities.Databricks;
using SAT_API.Infrastructure;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// ====== DETECTAR SI ESTAMOS EN AZURE APP SERVICE ======
var isAzureAppService = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
var siteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") ?? Assembly.GetExecutingAssembly().GetName().Name;

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configure Serilog
Log.Logger = ConfigureLogging(isAzureAppService);
builder.Logging.ClearProviders();
builder.Host.UseSerilog();

// registrar Dapper para usar snake_case automáticamente
DapperConfig.Configure();

// Add services to the container.
builder.Configuration.Sources.Clear();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Configurar la inyección de dependencias
builder.Services.AddSettings(builder.Configuration);
// Infrastructure Layer
builder.Services.AddInfrastructure();

// ===== APPLICATION LAYER (YA INCLUYE AUTENTICACIÓN) =====
// Tu DependencyInjection.cs ya configura AddAuthentication().AddBearerToken()
builder.Services.AddApplication();

// ===== SOLO AGREGAR POLÍTICAS DE AUTORIZACIÓN (SIN DUPLICAR AUTENTICACIÓN) =====
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger
ConfigureSwagger(builder.Services);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
    .AddCheck("databricks", () =>
    {
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Databricks connection OK");
    });

var app = builder.Build();

try
{
    Log.Information("Iniciando SAT API en {Environment} - Azure: {IsAzure} - Site: {SiteName}",
        app.Environment.EnvironmentName, isAzureAppService, siteName);

    // ====== MIDDLEWARES (ORDEN IMPORTANTE) ======

    // 1. Health Checks
    app.UseHealthChecks("/health");

    // 2. Middlewares personalizados (tu método de extensión)
    app.AddMiddleware(); // Esto incluye tus exception middlewares

    // 3. Request Logging
    ConfigureRequestLogging(app, isAzureAppService);

    // 4. HTTPS Redirection
    if (isAzureAppService || app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    // 5. CORS
    app.UseCors("AllowAll");
    
    // 6. ===== AUTENTICACIÓN Y AUTORIZACIÓN (ORDEN CRÍTICO) =====
    app.UseAuthentication();  // DEBE IR ANTES
    app.UseAuthorization();   // DEBE IR DESPUÉS

    // 7. Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservicio.Productos v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Microservicio.Productos - API SAT con JWT";
        c.DisplayRequestDuration();
        c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete);
        c.EnablePersistAuthorization();
        c.EnableDeepLinking();
        c.DisplayOperationId();
    });

    // 8. Redirección root a swagger
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

    app.MapGet("/api/info", () => new
    {
        Application = "Microservicio.Productos",
        Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
        Environment = app.Environment.EnvironmentName,
        IsAzure = isAzureAppService,
        SiteName = siteName,
        Timestamp = DateTime.UtcNow
    }).ExcludeFromDescription();

    // 9. Controllers
    app.MapControllers();

    Log.Information("SAT API configurada exitosamente");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación {Name} falló al iniciar", Assembly.GetExecutingAssembly().GetName().Name);
    return;
}
finally
{
    Log.Information("Cerrando {Name}", Assembly.GetExecutingAssembly().GetName().Name);
    await Log.CloseAndFlushAsync();
}

// ====== HELPER METHODS ======

static Serilog.ILogger ConfigureLogging(bool isAzureAppService)
{
    var loggerConfig = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", Assembly.GetExecutingAssembly().GetName().Name)
        .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
        .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0");

    if (isAzureAppService)
    {
        ConfigureAzureLogging(loggerConfig);
    }
    else
    {
        ConfigureLocalLogging(loggerConfig);
    }

    return loggerConfig.CreateLogger();
}

static void ConfigureAzureLogging(LoggerConfiguration loggerConfig)
{
    loggerConfig.WriteTo.Console(
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext:l}] {Message:lj}{NewLine}{Exception}");
    
    loggerConfig.WriteTo.File(
        path: "/home/LogFiles/Application/app-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 10,
        fileSizeLimitBytes: 25_000_000,
        rollOnFileSizeLimit: true,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1),
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext:l}] {Message:lj} {Properties:j}{NewLine}{Exception}");
    
    loggerConfig.WriteTo.File(
        path: "/home/LogFiles/Application/errors-.txt",
        restrictedToMinimumLevel: LogEventLevel.Error,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1),
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext:l}] {Message:lj} {Properties:j}{NewLine}{Exception}");
    
    loggerConfig.WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("RequestPath") || e.MessageTemplate.Text.Contains("HTTP"))
        .WriteTo.File(
            path: "/home/LogFiles/Application/requests-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 5,
            shared: true,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {Method} {RequestPath} {StatusCode} {Elapsed}ms {ClientIP} {Properties:j}{NewLine}"));
}

static void ConfigureLocalLogging(LoggerConfiguration loggerConfig)
{
    loggerConfig
        .MinimumLevel.Debug()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/app-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            fileSizeLimitBytes: 50_000_000,
            rollOnFileSizeLimit: true,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/errors-.txt",
            restrictedToMinimumLevel: LogEventLevel.Error,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 90,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("RequestPath"))
            .WriteTo.File(
                path: "logs/requests-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {Method} {RequestPath} {StatusCode} {Elapsed}ms {Properties:j}{NewLine}"));
}

static void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Microservicio.Productos",
            Version = "v1",
            Description = "Servicio API REST para consumir Databricks con autenticación JWT del SAT",
            Contact = new OpenApiContact
            {
                Name = "SAT API Team",
                Email = "eavillamil@bside.com.mx"
            }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header del SAT usando Bearer scheme. Ejemplo: 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
               Array.Empty<string>()
            }
        });
    });
}

static void ConfigureRequestLogging(WebApplication app, bool isAzureAppService)
{
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (ex != null)
            {
                return LogEventLevel.Error;
            }

            return elapsed > 5000 ? LogEventLevel.Warning : LogEventLevel.Information;
        };

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            EnrichDiagnosticContext(diagnosticContext, httpContext, isAzureAppService);
        };
    });
}

static void EnrichDiagnosticContext(Serilog.IDiagnosticContext diagnosticContext, HttpContext httpContext, bool isAzureAppService)
{
    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault() ?? string.Empty);
    diagnosticContext.Set("RequestPath", httpContext.Request.Path);
    diagnosticContext.Set("Method", httpContext.Request.Method);

    var clientIP = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                  ?? httpContext.Request.Headers["X-Azure-ClientIP"].FirstOrDefault()
                  ?? httpContext.Connection.RemoteIpAddress?.ToString();
    diagnosticContext.Set("ClientIP", clientIP ?? string.Empty);

    if (isAzureAppService)
    {
        diagnosticContext.Set("AzureRequestId", httpContext.Request.Headers["X-MS-RequestId"].FirstOrDefault() ?? string.Empty);
    }
}