using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Process;
namespace SAT_API.Domain.Interfaces;

public interface IEjecucionRepository
{
    Task<IEnumerable<Ejecucion>> ObtenerEjecucionesAsync();

    Task<IEnumerable<Ejecucion>> ObtenerEjecucionPorIdAsync(int id);

    Task<int> InsertarEjecucionAsync(Ejecucion ejecucion);

    Task<int> ActualizarNombreAsync(int id, string nuevoNombre, string nuevaFecha);
    Task<IntersectProcessStatusResponse> CruceInformacionEstatus(IntersectProcessStatusRequest request);
    Task<List<int>> ActualizarEstatusEjecucionIdAsync(int ejecucionId, int estatusEjecucionId, string comentarios);
}
