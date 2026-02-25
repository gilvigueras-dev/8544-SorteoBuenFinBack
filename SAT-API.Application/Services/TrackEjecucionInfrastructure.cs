using AutoMapper;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services
{
    /// <summary>
    /// Implementation of ITrackEjecucionInfrastructure facade for TrackEjecucionServices dependencies
    /// </summary>
    public class TrackEjecucionInfrastructure : ITrackEjecucionInfrastructure
    {
        public IMapper Mapper { get; }
        public ITranslator Translator { get; }
        public IAddressClientService AddressClientService { get; }
        public IDatabricksRepository DatabricksRepository { get; }
        public IJobConfigRepository JobConfigRepository { get; }
        public IValidationService ValidationService { get; }
        public IRoleManagementRepository RoleManagementRepository { get; }

        public TrackEjecucionInfrastructure(
            IMapper mapper,
            ITranslator translator,
            IAddressClientService addressClientService,
            IDatabricksRepository databricksRepository,
            IJobConfigRepository jobConfigRepository,
            IValidationService validationService,
            IRoleManagementRepository roleManagementRepository)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Translator = translator ?? throw new ArgumentNullException(nameof(translator));
            AddressClientService = addressClientService ?? throw new ArgumentNullException(nameof(addressClientService));
            DatabricksRepository = databricksRepository ?? throw new ArgumentNullException(nameof(databricksRepository));
            JobConfigRepository = jobConfigRepository ?? throw new ArgumentNullException(nameof(jobConfigRepository));
            ValidationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            RoleManagementRepository = roleManagementRepository ?? throw new ArgumentNullException(nameof(roleManagementRepository));
        }
    }
}
