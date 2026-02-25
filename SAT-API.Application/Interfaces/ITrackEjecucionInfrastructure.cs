using SAT_API.Domain.Interfaces.UserOperations;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using AutoMapper;

namespace SAT_API.Application.Interfaces
{
    /// <summary>
    /// Facade interface that wraps utility/infrastructure dependencies for TrackEjecucionServices
    /// </summary>
    public interface ITrackEjecucionInfrastructure
    {
        IMapper Mapper { get; }
        ITranslator Translator { get; }
        IAddressClientService AddressClientService { get; }
        IDatabricksRepository DatabricksRepository { get; }
        IJobConfigRepository JobConfigRepository { get; }
        IValidationService ValidationService { get; }
        IRoleManagementRepository RoleManagementRepository { get; }
    }
}
