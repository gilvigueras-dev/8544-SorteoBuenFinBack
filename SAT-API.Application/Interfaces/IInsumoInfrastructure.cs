using SAT_API.Application.Interfaces;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces.UserOperations;
using AutoMapper;

namespace SAT_API.Application.Interfaces
{
    /// <summary>
    /// Facade interface that wraps utility/infrastructure dependencies for InsumoService
    /// </summary>
    public interface IInsumoInfrastructure
    {
        IMapper Mapper { get; }
        ITranslator Translator { get; }
        IAddressClientService AddressClientService { get; }
        IDatabricksRepository DatabricksRepository { get; }
        IJobConfigService JobConfigService { get; }
        ITrackEjecucionRepository TrackEjecucionRepository { get; }
        IRoleManagementRepository RoleManagementRepository { get; }
    }
}
