using Dapper;
using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Track;
using SAT_API.Domain.Interfaces;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories;

public class TrackEjecucionRepository : ITrackEjecucionRepository
{
    private readonly IDbContext _context;

    public TrackEjecucionRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<List<int>> InsertarTrackEjecucionAsync(int ejecucionId, int estatusEjecucionId)
    {
        var query = $"SELECT {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_actualiza_track_ejecucion(@ejecucionId, @estatusEjecucionId, @comentarios)";
        using var connection = _context.CreateConnection();
        var nuevoTrackid = await connection.QueryAsync<int>(query, new
        {
            ejecucionId = ejecucionId,
            estatusEjecucionId = estatusEjecucionId,
            comentarios = ""
        });
        return nuevoTrackid.ToList();
    }

    public async Task<List<TrackEjecucion>> ObtenerTrackEjecucionAsync(int ejecucionId)
    {
        var query = $"SELECT track_id AS TrackId, ejecucion_id AS EjecucionId, estatus_ejecucion_id AS EstatusEjecucionId, fecha_inicio AS FechaCreacion, fecha_cambio AS FechaModificacion FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_track_ejecucion(@EjecucionId)";
        using var connection = _context.CreateConnection();
        var nuevoTrackid = await connection.QueryAsync<TrackEjecucion>(query, new
        {
            EjecucionId = ejecucionId
        });
        return nuevoTrackid.ToList();
    }

    public async Task<DataPopulationValidationResponse?> DataPopulationValidation(DataPopulationValidationRequest request)
    {
        var query = $"select estatus, estatus_valor from {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_validacion_poblado(@p_id_ejecucion, @p_id_etapa)";
        using var connection = _context.CreateConnection();
        var result = await connection.GetFirstOrDefaultAsync<DataPopulationValidationResponse, DataPopulationValidationRequest>(
            query,
            request
        );

        return result;
    }

}
