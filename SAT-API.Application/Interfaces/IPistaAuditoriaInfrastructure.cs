using SAT_API.Application.Interfaces;
using SAT_API.Application.Interfaces.Excel;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using AutoMapper;

namespace SAT_API.Application.Interfaces
{
    /// <summary>
    /// Facade interface that wraps utility/infrastructure dependencies for PistaAuditoriaServices
    /// </summary>
    public interface IPistaAuditoriaInfrastructure
    {
        ITranslator Translator { get; }
        IMapper Mapper { get; }
        IExcelService ExcelService { get; }
        IValidationService ValidationService { get; }
        IAddressClientService AddressClientService { get; }
    }
}
