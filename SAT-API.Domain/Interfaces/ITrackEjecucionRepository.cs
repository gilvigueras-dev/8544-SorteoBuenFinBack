using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Track;

namespace SAT_API.Domain.Interfaces;

public interface ITrackEjecucionRepository
{
    Task<List<int>> InsertarTrackEjecucionAsync(int ejecucionId, int estatusEjecucionId);
    Task<List<TrackEjecucion>> ObtenerTrackEjecucionAsync(int ejecucionId);
    Task<DataPopulationValidationResponse?> DataPopulationValidation(DataPopulationValidationRequest request);
}
