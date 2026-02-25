using FluentValidation;
using FluentValidation.Results;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using SAT_API.Application.Middlewares.Validators.Models;

namespace SAT_API.Application.Middlewares.Validators;

public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ValidationResponse<T>> ValidateAsync<T>(T entity)
        {
            var validator = GetValidator<T>();
            if (validator == null)
            {
                throw new InvalidOperationException($"No hay validador registrado para el tipo {typeof(T).Name}");
            }

            return await validator.ValidateWithResponseAsync(entity);
        }

        public async Task<ValidationResult> ValidateSimpleAsync<T>(T entity)
        {
            var validator = GetValidator<T>();
            if (validator == null)
            {
                throw new InvalidOperationException($"No hay validador registrado para el tipo {typeof(T).Name}");
            }

            return await validator.ValidateAsync(entity);
        }
        
        public async Task ValidateAndThrowAsync<T>(T entity)
        {
            var validator = GetValidator<T>();
            if (validator == null)
            {
                throw new InvalidOperationException($"No hay validador registrado para el tipo {typeof(T).Name}");
            }
            var result = await validator.ValidateAsync(entity);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

         public bool HasValidatorFor<T>()
    {
        return _serviceProvider.GetService(typeof(IValidator<T>)) != null;
    }

        public IValidator<T>? GetValidator<T>()
        {
            return _serviceProvider.GetService(typeof(IValidator<T>)) as IValidator<T>;
        }
    }