using AutoMapper;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Interfaces.Products;
using SAT_API.Application.Middlewares.Validators.Interfaces;

namespace SAT_API.Application.Services.Products
{
    /// <summary>
    /// Implementation of IProductsInfrastructure facade for ProductsService dependencies
    /// </summary>
    public class ProductsInfrastructure : IProductsInfrastructure
    {
        public IMapper Mapper { get; }
        public IValidationService ValidationService { get; }
        public ITranslator Translator { get; }
        public IAddressClientService AddressClientService { get; }

        public ProductsInfrastructure(
            IMapper mapper,
            IValidationService validationService,
            ITranslator translator,
            IAddressClientService addressClientService)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            ValidationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            Translator = translator ?? throw new ArgumentNullException(nameof(translator));
            AddressClientService = addressClientService ?? throw new ArgumentNullException(nameof(addressClientService));
        }
    }
}
