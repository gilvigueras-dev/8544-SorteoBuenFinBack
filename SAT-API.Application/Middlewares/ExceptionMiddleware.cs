using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SAT_API.Application.Common;
using SAT_API.Domain.Common;
using System.Net;
using System.Text.Json;

namespace SAT_API.Application.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var result = await HandleExceptionAsync(context, ex);
            
            if (!result.IsSuccess)
            {
                _logger.LogError(ex, "Error adicional procesando excepción: {Message}", result.Message);
            }
        }
    }

    private async Task<Result<ErrorResponse>> HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (errorResponse, errorInfos) = CreateErrorResponse(exception);
        
        context.Response.StatusCode = errorResponse.StatusCode;

        // Crear el Result que se escribirá como respuesta
        var result = Result<ErrorResponse>.Failure(
            message: errorResponse.Message,
            errors: errorInfos
        );

        // Serializar el Result completo como respuesta
        var jsonResponse = JsonSerializer.Serialize(result, s_jsonOptions);

        await context.Response.WriteAsync(jsonResponse);

        return result;
    }

    private (ErrorResponse errorResponse, List<ErrorInfo> errorInfos) CreateErrorResponse(Exception exception)
    {
        var errorResponse = new ErrorResponse();
        var errorInfos = new List<ErrorInfo>();

        var (statusCode, message, details, code, logLevel) = exception switch
        {
            ArgumentNullException ex => (
                HttpStatusCode.BadRequest,
                "Parámetro requerido faltante",
                ex.Message,
                "ARGUMENT_NULL",
                LogLevel.Warning),

            ArgumentException ex => (
                HttpStatusCode.BadRequest,
                "Parámetro inválido",
                ex.Message,
                "ARGUMENT_INVALID",
                LogLevel.Warning),

            UnauthorizedAccessException ex => (
                HttpStatusCode.Unauthorized,
                "No autorizado",
                ex.Message,
                "UNAUTHORIZED",
                LogLevel.Warning),

            KeyNotFoundException ex => (
                HttpStatusCode.NotFound,
                "Recurso no encontrado",
                ex.Message,
                "NOT_FOUND",
                LogLevel.Information),

            InvalidOperationException ex => (
                HttpStatusCode.BadRequest,
                "Operación inválida",
                ex.Message,
                "INVALID_OPERATION",
                LogLevel.Warning),

            BusinessException ex => (
                HttpStatusCode.BadRequest,
                ex.Message,
                ex.Details,
                "BUSINESS_ERROR",
                LogLevel.Warning),

            _ => (
                HttpStatusCode.InternalServerError,
                exception.Message,
                exception.StackTrace ?? string.Empty,
                "INTERNAL_ERROR",
                LogLevel.Error)
        };

        // Log con el nivel apropiado
        _logger.Log(logLevel, exception, "Excepción procesada: {ExceptionType} - {Message}", 
            exception.GetType().Name, exception.Message);

        errorResponse.StatusName = statusCode.ToString();
        errorResponse.StatusCode = (int)statusCode;
        errorResponse.Message = message;
        errorResponse.Details = details;

        errorInfos.Add(new ErrorInfo
        {
            Code = code,
            Message = exception.Message
        });

        return (errorResponse, errorInfos);
    }
}

/// <summary>
/// Respuesta de error estándar del sistema
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Nombre descriptivo del estado del error
    /// </summary>
    /// <example>Bad Request</example>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// Código de estado HTTP
    /// </summary>
    /// <example>400</example>
    public int StatusCode { get; set; }

    /// <summary>
    /// Mensaje descriptivo del error
    /// </summary>
    /// <example>Error en la validación de los datos</example>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detalles adicionales del error
    /// </summary>
    /// <example>El campo NumberInJob es requerido</example>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora cuando ocurrió el error (UTC)
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Excepción personalizada para errores de negocio
/// </summary>
public class BusinessException : Exception
{
    public string Details { get; }

    public BusinessException(string message, string details = "") : base(message)
    {
        Details = details;
    }
}