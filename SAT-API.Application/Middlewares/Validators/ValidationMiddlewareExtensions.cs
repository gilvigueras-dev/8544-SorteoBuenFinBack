using FluentValidation;
using SAT_API.Application.Middlewares.Validators.Models;

namespace SAT_API.Application.Middlewares;

public static class ValidationMiddlewareExtensions
{
    /// <summary>
    /// Extensión que permite obtener ValidationResponse desde cualquier IValidator
    /// </summary>
    public static async Task<ValidationResponse<T>> ValidateWithResponseAsync<T>(
        this IValidator<T> validator,
        T entity)
    {
        var validationResult = await validator.ValidateAsync(entity);

        return new ValidationResponse<T>
        {
            Entity = entity,
            IsValid = validationResult.IsValid,
            Errors = validationResult.Errors.Select(e => new ValidationError
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString(),
                ErrorCode = e.ErrorCode
            }).ToList(),
            ValidationResult = validationResult,
            ValidationTimestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Extensión que permite validar y lanzar una excepción si no es válida
    /// </summary>
    public static async Task ValidateAndThrowAsync<T>(
        this IValidator<T> validator,
        T entity)
    {
        var validationResult = await validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}
