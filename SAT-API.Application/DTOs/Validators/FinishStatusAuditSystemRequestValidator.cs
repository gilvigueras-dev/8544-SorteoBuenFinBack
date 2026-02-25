using FluentValidation;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Middlewares.Validators;

namespace SAT_API.Application.DTOs.Validators;

public class FinishStatusAuditSystemRequestValidator: BaseValidator<FinishStatusAuditSystemRequestDto>
{   
    public FinishStatusAuditSystemRequestValidator(ITranslator translator)
    {
        ITranslator _translator = translator ?? throw new ArgumentNullException(nameof(translator));

        RuleFor(x => x.AuditSystemId)
            .GreaterThan(0)
            .WithMessage(_translator["AuditSystemIdMustBeGreaterThanZero"]);
    }
}
