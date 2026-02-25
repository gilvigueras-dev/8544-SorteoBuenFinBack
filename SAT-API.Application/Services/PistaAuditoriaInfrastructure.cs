using AutoMapper;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Interfaces.Excel;
using SAT_API.Application.Middlewares.Validators.Interfaces;

namespace SAT_API.Application.Services
{
    /// <summary>
    /// Implementation of IPistaAuditoriaInfrastructure facade for PistaAuditoriaServices dependencies
    /// </summary>
    public class PistaAuditoriaInfrastructure : IPistaAuditoriaInfrastructure
    {
        public ITranslator Translator { get; }
        public IMapper Mapper { get; }
        public IExcelService ExcelService { get; }
        public IValidationService ValidationService { get; }
        public IAddressClientService AddressClientService { get; }

        public PistaAuditoriaInfrastructure(
            ITranslator translator,
            IMapper mapper,
            IExcelService excelService,
            IValidationService validationService,
            IAddressClientService addressClientService)
        {
            Translator = translator ?? throw new ArgumentNullException(nameof(translator));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            ExcelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
            ValidationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            AddressClientService = addressClientService ?? throw new ArgumentNullException(nameof(addressClientService));
        }
    }
}
