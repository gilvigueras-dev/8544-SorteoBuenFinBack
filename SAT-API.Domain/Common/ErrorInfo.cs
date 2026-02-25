namespace SAT_API.Domain.Common
{
    /// <summary>
    /// Información de error estándar
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// Código del error
        /// </summary>
        /// <example>INSUMO_NOT_FOUND</example>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Mensaje descriptivo del error
        /// </summary>
        /// <example>El insumo especificado no fue encontrado</example>
        public string Message { get; set; } = string.Empty;
    }
}
