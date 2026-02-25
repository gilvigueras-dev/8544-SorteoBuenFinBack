using SAT_API.Application.Interfaces;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using AutoMapper;

namespace SAT_API.Application.Interfaces.Products
{
    /// <summary>
    /// Facade interface that wraps utility/infrastructure dependencies for ProductsService
    /// </summary>
    public interface IProductsInfrastructure
    {
        IMapper Mapper { get; }
        IValidationService ValidationService { get; }
        ITranslator Translator { get; }
        IAddressClientService AddressClientService { get; }
    }
}
