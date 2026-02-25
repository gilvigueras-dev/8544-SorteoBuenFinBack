using FluentValidation;
using SAT_API.Application.DTOs.Track;
using SAT_API.Application.Middlewares.Validators;

namespace SAT_API.Application.DTOs.Validators;

public class DataPopulationRequestValidator:BaseValidator<DataPopulationRequestDto>
{
    public DataPopulationRequestValidator()
    {
        RuleFor(x => x.StageId)
            .GreaterThan(0)
            .WithMessage("StageId must be greater than 0.");
    }
}

