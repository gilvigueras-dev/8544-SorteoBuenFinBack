using AutoMapper;
using SAT_API.Application.Interfaces;

namespace SAT_API.Application.Interfaces
{
    /// <summary>
    /// Facade interface that wraps infrastructure/utility dependencies for EjecucionService
    /// </summary>
    public interface IEjecucionInfrastructure
    {
        IMapper Mapper { get; }
        ITranslator Translator { get; }
        IAddressClientService AddressClientService { get; }
    }
}
