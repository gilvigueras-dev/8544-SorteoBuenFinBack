using FluentValidation;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.Middlewares.Validators;

namespace SAT_API.Application.DTOs.Validators;

public class ProductStatusRequestValidator: BaseValidator<ProductStatusRequestDto>
{
    public ProductStatusRequestValidator()
    {
        RuleFor(x => x.IdArchivo)
            .GreaterThan(0)
            .WithMessage("IdArchivo cannot be empty.");

        RuleFor(x => x.IdEjecucion)
            .GreaterThan(0)
            .WithMessage("IdEjecucion must be greater than 0.");
    }
}

