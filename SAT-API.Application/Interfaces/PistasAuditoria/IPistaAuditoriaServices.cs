using SAT_API.Application.Common;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.DTOs.UserOperations;
using SAT_API.Domain.Entities.Products;

namespace SAT_API.Application.Interfaces;

public interface IPistaAuditoriaServices
{
    Task<Result<List<InsertarPistaAuditoriaDto>>> InsertarPistaAuditoriaAsync(InsertarPistaAuditoriaDto request);

    Task<Result<string>> InsertSystemAudiAsync(SystemAuditRequestDto request);

    Task<Result<byte[]>> GetAuditTrailsAsync(ProductsExcelExportRequestDto request);

    Task<Result<List<ActionsCatalogResponse>>> GetActionsCatalogAsync();
    Task<Result<string>> SetFinishedAuditAsync(FinishStatusAuditSystemRequestDto request);
    Task<Result<string>> UpdateSystemAuditAsync(UpdateSystemAuditRequestDto request);
    Task<Result<List<StageCatalogResponseDto>>> GetStagesCatalogAsync();
}