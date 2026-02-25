using SAT_API.Domain.Entities;

namespace SAT_API.Domain.Interfaces;

public interface IInsumoRepository
{
    Task<IEnumerable<Insumo>> GetInsumosAsync(int ejecucionId, int etapa);

    Task<IEnumerable<EstatusValidacion>> GetEstatusValidacionAsync(int ejecucionId, int etapa);
}
