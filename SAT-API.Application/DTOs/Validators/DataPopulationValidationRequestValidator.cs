using FluentValidation;
using SAT_API.Application.DTOs.Track;
using SAT_API.Application.Middlewares.Validators;

namespace SAT_API.Application.DTOs.Validators;

public class DataPopulationValidationRequestValidator:BaseValidator<DataPopulationValidationRequestDto>
{
    public DataPopulationValidationRequestValidator()
    {
        RuleFor(x => x.ExecutionId)
            .GreaterThan(0)
            .WithMessage("ExecutionId must be greater than 0.");

        RuleFor(x => x.StageId)
            .GreaterThan(0)
            .WithMessage("StageId must be greater than 0.");
    }
}
