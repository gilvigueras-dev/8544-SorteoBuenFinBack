using AutoMapper;
using SAT_API.Application.Interfaces;

namespace SAT_API.Application.Services
{
    /// <summary>
    /// Implementation of IEjecucionInfrastructure facade for EjecucionService dependencies
    /// </summary>
    public class EjecucionInfrastructure : IEjecucionInfrastructure
    {
        public IMapper Mapper { get; }
        public ITranslator Translator { get; }
        public IAddressClientService AddressClientService { get; }

        public EjecucionInfrastructure(IMapper mapper, ITranslator translator, IAddressClientService addressClientService)
        {
            Mapper = mapper;
            Translator = translator;
            AddressClientService = addressClientService;
        }
    }
}
