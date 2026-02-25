using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SAT_API.Application.Common;
using SAT_API.Domain.Common;

namespace SAT_API.Application.Middlewares
{
    public class ExceptionEnvironmentMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionEnvironmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (IsEnvironmentException(ex))
                {
                    return;
                }
                
                // Redirigir a ExceptionMiddleware si no es una excepción de entorno
                throw;
            }
        }

        private static bool IsEnvironmentException(Exception exception)
        {
            return exception is EnvironmentFileNotFoundException
                || exception is EnvironmentVariableNotFoundException
                || exception is EnvironmentFileCorruptedException
                || exception is EnvironmentVariableFormatException
                || (exception is UnauthorizedAccessException && exception.Message.Contains(".env"))
                || (exception is IOException && exception.Message.Contains(".env"));
        }
    }

    public class EnvironmentFileNotFoundException : Exception
    {
        public string FilePath { get; }

        public EnvironmentFileNotFoundException(string filePath)
            : base($"El archivo .env no fue encontrado en la ruta: {filePath}")
        {
            FilePath = filePath;
        }

        public EnvironmentFileNotFoundException(string filePath, Exception innerException)
            : base($"El archivo .env no fue encontrado en la ruta: {filePath}", innerException)
        {
            FilePath = filePath;
        }
    }

    public class EnvironmentVariableNotFoundException : Exception
    {
        public string VariableName { get; }

        public EnvironmentVariableNotFoundException(string variableName)
            : base($"La variable de entorno '{variableName}' es requerida pero no fue encontrada")
        {
            VariableName = variableName;
        }

        public EnvironmentVariableNotFoundException(string variableName, Exception innerException)
            : base($"La variable de entorno '{variableName}' es requerida pero no fue encontrada", innerException)
        {
            VariableName = variableName;
        }
    }

    public class EnvironmentFileCorruptedException : Exception
    {
        public string FilePath { get; }
        public int LineNumber { get; }
        public string LineContent { get; }

        public EnvironmentFileCorruptedException(string filePath, int lineNumber, string lineContent)
            : base($"El archivo .env está corrupto en la línea {lineNumber}: {lineContent}")
        {
            FilePath = filePath;
            LineNumber = lineNumber;
            LineContent = lineContent;
        }

        public EnvironmentFileCorruptedException(string filePath, int lineNumber, string lineContent, Exception innerException)
            : base($"El archivo .env está corrupto en la línea {lineNumber}: {lineContent}", innerException)
        {
            FilePath = filePath;
            LineNumber = lineNumber;
            LineContent = lineContent;
        }
    }

    public class EnvironmentVariableFormatException : Exception
    {
        public string VariableName { get; }
        public string ExpectedFormat { get; }

        public EnvironmentVariableFormatException(string variableName, string expectedFormat)
            : base($"La variable '{variableName}' tiene un formato inválido. Formato esperado: {expectedFormat}")
        {
            VariableName = variableName;
            ExpectedFormat = expectedFormat;
        }

        public EnvironmentVariableFormatException(string variableName, string expectedFormat, Exception innerException)
            : base($"La variable '{variableName}' tiene un formato inválido. Formato esperado: {expectedFormat}", innerException)
        {
            VariableName = variableName;
            ExpectedFormat = expectedFormat;
        }
    }
}