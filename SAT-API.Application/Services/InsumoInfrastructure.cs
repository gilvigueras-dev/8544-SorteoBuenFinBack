using AutoMapper;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services
{
    /// <summary>
    /// Implementation of IInsumoInfrastructure facade for InsumoService dependencies
    /// </summary>
    public class InsumoInfrastructure : IInsumoInfrastructure
    {
        public IMapper Mapper { get; }
        public ITranslator Translator { get; }
        public IAddressClientService AddressClientService { get; }
        public IDatabricksRepository DatabricksRepository { get; }
        public IJobConfigService JobConfigService { get; }
        public ITrackEjecucionRepository TrackEjecucionRepository { get; }
        public IRoleManagementRepository RoleManagementRepository { get; }

        public InsumoInfrastructure(
            IMapper mapper,
            ITranslator translator,
            IAddressClientService addressClientService,
            IDatabricksRepository databricksRepository,
            IJobConfigService jobConfigService,
            ITrackEjecucionRepository trackEjecucionRepository,
            IRoleManagementRepository roleManagementRepository)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Translator = translator ?? throw new ArgumentNullException(nameof(translator));
            AddressClientService = addressClientService ?? throw new ArgumentNullException(nameof(addressClientService));
            DatabricksRepository = databricksRepository ?? throw new ArgumentNullException(nameof(databricksRepository));
            JobConfigService = jobConfigService ?? throw new ArgumentNullException(nameof(jobConfigService));
            TrackEjecucionRepository = trackEjecucionRepository ?? throw new ArgumentNullException(nameof(trackEjecucionRepository));
            RoleManagementRepository = roleManagementRepository ?? throw new ArgumentNullException(nameof(roleManagementRepository));
        }
    }
}
