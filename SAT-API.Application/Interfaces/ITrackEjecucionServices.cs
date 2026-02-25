using SAT_API.Application.Common;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Ejecuciones;
using SAT_API.Application.DTOs.Track;

namespace SAT_API.Application.Interfaces;

public interface ITrackEjecucionServices
{
    Task<Result<List<InsertarTrackEjecucionDto>>> InsertarTrackEjecucionAsync(InsertarTrackEjecucionRequestDto request);
    Task<Result<List<TrackEjecucionDto>>> ObtenerTrackEjecucionAsync(int ejecucionId);
    Task<Result<DataPopulationValidationResponseDto?>> DataPopulationValidation(DataPopulationValidationRequestDto request);
    Task<Result<RunJobResponseDto>> DataPopulation(DataPopulationRequestDto request);
}
