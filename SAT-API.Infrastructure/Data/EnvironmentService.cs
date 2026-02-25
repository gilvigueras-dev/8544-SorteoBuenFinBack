using Microsoft.Extensions.Logging;
using SAT_API.Application.Middlewares;

namespace SAT_API.Infrastructure.Data
{
    public class EnvironmentService
    {
        private readonly ILogger<EnvironmentService> _logger;
        private readonly string _envFilePath = string.Empty;

        public EnvironmentService(ILogger<EnvironmentService> logger)
        {
            _logger = logger;

            if (!Path.Exists($"{Directory.GetCurrentDirectory()}/.env"))
            {
                return;
            }
            _envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        }

        public void LoadEnvironmentVariables()
        {
            try
            {
                if (!File.Exists(_envFilePath))
                {
                    throw new EnvironmentFileNotFoundException(_envFilePath);
                }

                var lines = File.ReadAllLines(_envFilePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();

                    if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
                        continue;

                    if (!line.Contains('='))
                    {
                        throw new EnvironmentFileCorruptedException(_envFilePath, i + 1, line);
                    }

                    var parts = line.Split('=', 2);
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    Environment.SetEnvironmentVariable(key, value);
                    _logger.LogDebug("Variable de entorno cargada: {Key}", key);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Sin permisos para leer el archivo .env: {_envFilePath}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Error al leer el archivo .env: {_envFilePath}", ex);
            }
        }

        public static string GetRequiredEnvironmentVariable(string variableName)
        {
            // Es buena práctica validar el nombre de la variable antes de buscarla
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentNullException(nameof(variableName));

            var value = Environment.GetEnvironmentVariable(variableName);

            if (string.IsNullOrEmpty(value))
            {
                throw new EnvironmentVariableNotFoundException(variableName);
            }

            // El reemplazo de caracteres se puede hacer de forma directa
            return value.Replace("\\", "");
        }

        public static T GetRequiredEnvironmentVariable<T>(string variableName, Func<string, T> converter)
        {
            var value = GetRequiredEnvironmentVariable(variableName);

            try
            {
                return converter(value);
            }
            catch (Exception ex)
            {
                throw new EnvironmentVariableFormatException(variableName, typeof(T).Name, ex);
            }
        }
    }
}

