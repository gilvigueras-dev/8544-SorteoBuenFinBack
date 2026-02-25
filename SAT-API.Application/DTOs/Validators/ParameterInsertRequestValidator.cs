using FluentValidation;
using SAT_API.Application.DTOs.Parameters;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Middlewares.Validators;

namespace SAT_API.Application.DTOs.Validators;

public class ParameterInsertRequestValidator : BaseValidator<ParameterInsertRequestDto>
{
    public ParameterInsertRequestValidator(ITranslator translator)
    {
        ITranslator _translator = translator;

        RuleFor(x => x.ExecutionId)
            .GreaterThan(0)
            .WithMessage(_translator["ApplicationParametersExecutionIdInvalid"]);

        RuleFor(x => x.Parameters)
            .NotNull()
            .WithMessage(_translator["ApplicationParametersEmpty"])
            .NotEmpty()
            .WithMessage(_translator["ApplicationParametersEmpty"]);
    }
}
