namespace SAT_API.Domain.Common
{
    /// <summary>
    /// Respuesta estándar de la API
    /// </summary>
    /// <typeparam name="T">Tipo de datos que contiene la respuesta</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        /// <example>true</example>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje general de la respuesta
        /// </summary>
        /// <example>Operación completada exitosamente</example>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Datos de la respuesta
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Lista de errores si los hay
        /// </summary>
        public List<ErrorInfo> Errors { get; set; } = new();

        // Métodos estáticos para crear respuestas
        public static ApiResponse<T> SuccessResponse(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new List<ErrorInfo>()
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<ErrorInfo>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<ErrorInfo>()
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, string errorCode, string errorMessage)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<ErrorInfo>
            {
                new ErrorInfo { Code = errorCode, Message = errorMessage }
            }
            };
        }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResponse(string message = "Operación exitosa")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Data = null,
                Errors = new List<ErrorInfo>()
            };
        }

        public new static ApiResponse ErrorResponse(string message, List<ErrorInfo>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Data = null,
                Errors = errors ?? new List<ErrorInfo>()
            };
        }
    }
}
