using SAT_API.Application.Common;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Insumos;

namespace SAT_API.Application.Interfaces;

public interface IInsumoService
{
    Task<Result<List<InsumoDto>>> GetInsumosEntradaAsync(int ejecucionId, int etapa);
    Task<Result<ProcesoValidacionDto>> GetValidarInsumos(int ejecucionId, int etapa);
    Task<Result<List<EstatusValidacionDto>>> GetEstatusValidacionAsync(int ejecucionId, int etapa);
    Task<Result<RunJobResponseDto>> GenerateStageResources(int ejecucionId, int etapa);
    Task<Result<List<InsumoDto>>> GetOutputResourcesAsync(int ejecucionId, int etapa);
    Task<Result<RunStatusJobResponseDto>> StatusJobResourceAsync(long runId);
}
