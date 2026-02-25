using SAT_API.Application.Common;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Ejecuciones;
using SAT_API.Application.DTOs.Process;

namespace SAT_API.Application.Interfaces;

public interface IEjecucionService
{
    Task<Result<List<EjecucionDto>>> ObtenerEjecucionesAsync();
    Task<Result<List<EjecucionDto>>> ObtenerEjecucionPorIdAsync(int id);
    Task<Result<List<InsertarEjecucionDto>>> InsertarEjecucionAsync(InsertarEjecucionRequestDto ejecucion);
    Task<Result<RunJobResponseDto>> SetEjecucionEtapa(int idEjecucion, int numeroEtapa);
    Task<Result<IntersectProcessStatusResponseDto>> CruceInformacionEstatus(IntersectProcessStatusRequestDto request);
    Task<Result<List<ActualizarEstatusEjecucionIdResponseDto>>> ActualizarEstatusEjecucionIdAsync(ActualizarEstatusEjecucionRequestDto request);
    Task<Result<bool>> MoverProductoEtapa(int etapa);
}
