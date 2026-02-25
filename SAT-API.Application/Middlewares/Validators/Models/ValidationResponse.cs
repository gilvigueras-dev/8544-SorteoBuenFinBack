using FluentValidation.Results;

namespace SAT_API.Application.Middlewares.Validators.Models;

 public class ValidationResponse<T>
    {
        public T? Entity { get; set; }
        public bool IsValid { get; set; }
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();
        public ValidationResult? ValidationResult { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public DateTime ValidationTimestamp { get; set; }

        /// <summary>
        /// Devuelve los errores agrupados por propiedad
        /// </summary>
        public Dictionary<string, List<string>> GetGroupedErrors()
        {
            return Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToList());
        }

        /// <summary>
        /// Devuelve el primer error de una propiedad específica
        /// </summary>
        public string GetFirstErrorForProperty(string propertyName)
        {
            return Errors.FirstOrDefault(e => e.PropertyName == propertyName)?.ErrorMessage ?? string.Empty;
        }

        /// <summary>
        /// Indica si una propiedad específica tiene errores
        /// </summary>
        public bool HasErrorsForProperty(string propertyName)
        {
            return Errors.Any(e => e.PropertyName == propertyName);
        }
    }

    public class ValidationError
    {
        public required string PropertyName { get; set; }
        public required string ErrorMessage { get; set; }
        public string? AttemptedValue { get; set; }
        public string? ErrorCode { get; set; }
    }
