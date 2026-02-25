using Dapper;
using SAT_API.Domain.Entities;
using SAT_API.Domain.Interfaces;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories;

public class InsumoRepository: IInsumoRepository
{
    #region Constructor
    private readonly IDbContext _context;

    public InsumoRepository(IDbContext context)
    {
        _context = context;
    } 
    #endregion

    public async Task<IEnumerable<Insumo>> GetInsumosAsync(int ejecucionId, int etapa)
    {
        var query = $"SELECT * from {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_get_insumoporetapa(@ejecucionId,@etapa)";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Insumo>(query, new { ejecucionId = ejecucionId, etapa = etapa });
    }

    public async Task<IEnumerable<EstatusValidacion>> GetEstatusValidacionAsync(int ejecucionId, int etapa)
    {
        var query = $"SELECT * from {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_get_insumoporetapa(@ejecucionId,@etapa)";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<EstatusValidacion>(query, new { ejecucionId = ejecucionId, etapa = etapa });
    } 
}
