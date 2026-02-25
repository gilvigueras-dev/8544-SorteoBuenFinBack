using FluentValidation;
using SAT_API.Application.Middlewares.Validators.Models;

namespace SAT_API.Application.Middlewares.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    /// <summary>
    /// Valida la entidad y devuelve un resultado personalizado
    /// </summary>
    public async Task<ValidationResponse<T>> ValidateWithResponseAsync(T entity)
    {
        var validationResult = await ValidateAsync(entity);

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
            ValidationResult = validationResult
        };
    }

    /// <summary>
    /// Valida la entidad y lanza una excepción si no es válida
    /// </summary>
    public async Task ValidateAndThrowAsync(T entity)
    {
        var validationResult = await ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}
