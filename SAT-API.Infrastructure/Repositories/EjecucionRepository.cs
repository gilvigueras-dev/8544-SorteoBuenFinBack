using Dapper;
using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Process;
using SAT_API.Domain.Interfaces;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories;

public class EjecucionRepository : IEjecucionRepository
{
    private readonly IDbContext _context;

    public EjecucionRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ejecucion>> ObtenerEjecucionesAsync()
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_ejecuciones(@solo_activas)";
        using var connection = _context.CreateConnection();
        var listaEjecucion = await connection.QueryAsync<Ejecucion>(query, new
        {
            solo_activas = true
        });
        return listaEjecucion.ToList();
    }

    public async Task<IEnumerable<Ejecucion>> ObtenerEjecucionPorIdAsync(int id)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_ejecucion(@p_ejecucion_id)";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Ejecucion>(query, new { p_ejecucion_id = id });
    }

    public async Task<int> InsertarEjecucionAsync(Ejecucion ejecucion)
    {
        var sql = $"SELECT {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_insertar_ejecucion(@p_nombre::VARCHAR, @p_fecha::TIMESTAMP, @p_id_estatus_ejecucion::INTEGER)";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, new { p_nombre = ejecucion.Nombre, p_fecha = DateTime.Now, p_id_estatus_ejecucion = ejecucion.IdEstatusEjecucion });
    }

    public async Task<int> ActualizarNombreAsync(int id, string nuevoNombre, string nuevaFecha)
    {
        var sql = $"UPDATE {ConstantsSchemas.dec08544_dd_sac_svsbf}.ejecucion SET nombre = @nombre, fecha=@fecha WHERE id = @id";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, new { id = id, nombre = nuevoNombre, fecha = nuevaFecha });
    }

    public async Task<IntersectProcessStatusResponse> CruceInformacionEstatus(IntersectProcessStatusRequest request)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_validacion_cruces(@p_id_ejecucion, @p_id_etapa)";

        using var connection = _context.CreateConnection();
        var result = await connection.GetFirstOrDefaultAsync<IntersectProcessStatusResponse,IntersectProcessStatusRequest>(query, request);
        
        if (result == null)
        {
            throw new InvalidOperationException("No se encontró información para el cruce de estatus.");
        }
        
        return result;
    }
    public async Task<List<int>> ActualizarEstatusEjecucionIdAsync(int ejecucionId, int estatusEjecucionId, string comentarios)
    {
        var query = $"SELECT {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_actualiza_track_ejecucion(@ejecucionId, @estatusEjecucionId, @comentarios)";
        using var connection = _context.CreateConnection();
        var nuevoTrackid = await connection.QueryAsync<int>(query, new
        {
            ejecucionId = ejecucionId,
            estatusEjecucionId = estatusEjecucionId,
            comentarios = comentarios

        });
        return nuevoTrackid.ToList();
    }
}
