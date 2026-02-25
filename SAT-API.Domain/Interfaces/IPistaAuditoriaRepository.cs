using SAT_API.Domain.Entities.PistasAuditoria;
using SAT_API.Domain.Entities.Products;
using SAT_API.Domain.Entities.UserOperations;

namespace SAT_API.Domain.Interfaces;

public interface IPistaAuditoriaRepository
{
    Task<int> InsertarTrackEjecucionAsync(int ejecucionId, string actividad, string descripcion);

    Task<string> InsertSystemAudiAsync(SystemAuditRequest request);

    Task<List<AuditTrailResponse>> GetAuditTrailsAsync(ProductsExcelExportRequest request);

    Task<List<ActionsCatalogResponse>> GetActionsCatalogAsync();
    Task<string> SetFinishedAuditAsync(FinishStatusAuditSystemRequest request);
    Task<string> UpdateSystemAuditAsync(UpdateSystemAuditRequest request);
    Task<List<StageCatalogResponse>> GetStagesCatalogAsync();
}