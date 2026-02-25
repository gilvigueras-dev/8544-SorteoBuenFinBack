using FluentValidation;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.Middlewares.Validators;

namespace SAT_API.Application.DTOs.Validators;

public class ControlNumbersRequestValidator: BaseValidator<ControlNumbersRequestDto>
{
    public ControlNumbersRequestValidator()
    {
        RuleFor(x => x.ExecutionId)
            .GreaterThan(0)
            .WithMessage("ExecutionId must be greater than 0.");

        RuleFor(x => x.StageId)
            .GreaterThan(0)
            .WithMessage("StageId must be greater than 0.");
    }
}
