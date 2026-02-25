using FluentValidation;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.Middlewares.Validators;

namespace SAT_API.Application.DTOs.Validators;

public class ProductRequestValidator: BaseValidator<ProductRequestDto>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.IdEjecucion)
            .GreaterThan(0)
            .WithMessage("IdEjecucion must be greater than 0.");

        RuleFor(x => x.IdEtapa)
            .GreaterThan(0)
            .WithMessage("IdEtapa must be greater than 0.");
    }
}