using FluentValidation;
using FluentValidation.Results;
using SAT_API.Application.Middlewares.Validators.Models;

namespace SAT_API.Application.Middlewares.Validators.Interfaces;

public interface IValidationService
{
  /// <summary>
  /// Valida una entidad de tipo T usando su IValidator registrado
  /// </summary>
  Task<ValidationResponse<T>> ValidateAsync<T>(T entity);

  /// <summary>
  /// Valida una entidad y devuelve solo el ValidationResult
  /// </summary>
  Task<ValidationResult> ValidateSimpleAsync<T>(T entity);

  /// <summary>
  /// Valida una entidad de tipo T y lanza una excepción si la validación falla
  /// </summary>
  /// <exception cref="ValidationException">Lanza una excepción si la validación falla</exception>
  /// <param name="entity">Entidad a validar</param>
  /// <returns>T</returns>
  /// <remarks>
  /// Este método es útil cuando se desea validar una entidad y manejar la excepción en un middleware
  /// o en un controlador, evitando la necesidad de manejar el resultado de validación manualmente
  /// </remarks>
  /// <example>
  /// await _validationService.ValidateAndThrowAsync(entity);
  /// </example>
  /// <typeparam name="T">Tipo de la entidad a validar</typeparam>
  /// <returns>T</returns>
  /// <exception cref="ValidationException">Lanza una excepción si la validación falla</exception>
  /// <remarks>
  Task ValidateAndThrowAsync<T>(T entity);

  /// <summary>
  /// Verifica si existe un validador para el tipo T
  /// </summary>
  bool HasValidatorFor<T>();

  /// <summary>
  /// Obtiene el validador para el tipo T
  /// </summary>
  IValidator<T>? GetValidator<T>();
}