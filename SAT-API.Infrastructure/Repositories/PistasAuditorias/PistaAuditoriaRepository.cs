using Dapper;
using SAT_API.Domain.Entities.PistasAuditoria;
using SAT_API.Domain.Entities.Products;
using SAT_API.Domain.Entities.UserOperations;
using SAT_API.Domain.Interfaces;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories;

public class PistaAuditoriaRepository : IPistaAuditoriaRepository
{
    private readonly IDbContext _context;

    public PistaAuditoriaRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<int> InsertarTrackEjecucionAsync(int ejecucionId, string actividad, string descripcion)
    {
        const string query = "select * from dec08544_dd_sac_svsbf.fn_insertar_pista_auditoria(@p_id_ejecucion,@p_actividad,@p_descripcion)";
        using var connection = _context.CreateConnection();
        var nuevoTrackid = await connection.ExecuteScalarAsync<int>(query, new
        {
            p_id_ejecucion = ejecucionId,
            p_actividad = actividad,
            p_descripcion = descripcion
        });
        return nuevoTrackid;
    }

    public async Task<string> InsertSystemAudiAsync(SystemAuditRequest request)
    {
        var query = @$"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_insertar_pista_auditoria(
                @p_id_ejecucion,
                @p_ambiente,
                @p_usuario,
                @p_mac_address_ip,
                @p_id_pista_auditoria,
                @p_etapa,
                @p_id_archivo
            )";
        using var connection = _context.CreateConnection();
        var result = await connection.GetFirstOrDefaultAsync<string, SystemAuditRequest>(query, request);
        return result ?? string.Empty;
    }

    public async Task<string> UpdateSystemAuditAsync(UpdateSystemAuditRequest request)
    {
        var query = @$"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_actualiza_final_pista_auditoria(
                @id
            )";
        using var connection = _context.CreateConnection();
        var result = await connection.GetFirstOrDefaultAsync<string, UpdateSystemAuditRequest>(query, request);
        return result ?? string.Empty;
    }

    public async Task<List<AuditTrailResponse>> GetAuditTrailsAsync(ProductsExcelExportRequest request)
    {
        var query = $@"
    SELECT
    ""IDEvento"" AS ""EventId"",
    ""Folio"",
    ""RFCC"" AS ""RFC"",
    ""Etapa"" AS ""Stage"",
    ""ID ejecución"" AS ""ExecutionId"",
    ""Fecha de Consulta Inicio"" AS ""StartQueryDate"",
    ""Fecha de Consulta Final"" AS ""EndQueryDate"",
    ""Accion"" AS ""Action"",
    ""Rol Archivo"" AS ""FileRole"",
    ""Nombre Archivo"" AS ""FileName"",
    ""URL"",
    ""IP/Identificador"" AS ""IPOrIdentifier""
FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_pistas_auditoria(
        @p_etapa,
        @p_fecha_inicio,
        @p_fecha_final,
        @p_rfc,
        @p_accion
    );";

        using var connection = _context.CreateConnection();
        var parameters = new
        {
            p_etapa = request.Stage,
            p_fecha_inicio = request.StartDate.HasValue ? request.StartDate : null,
            p_fecha_final = request.EndDate.HasValue ? request.EndDate : null,
            p_rfc = request.RFC ?? null,
            p_accion = request.Action ?? null
        };

        var results = await connection.QueryAsync<AuditTrailResponse>(query, parameters);
        return results?.ToList() ?? new List<AuditTrailResponse>();
    }

    public async Task<List<ActionsCatalogResponse>> GetActionsCatalogAsync()
    {
        var query = $@"
                    SELECT
                    *
                    FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_catalogo_pistas_auditoria();";

        using var connection = _context.CreateConnection();
        var results = await connection.QueryAsync<ActionsCatalogResponse>(query);
        return results?.ToList() ?? new List<ActionsCatalogResponse>();
    }

    public async Task<string> SetFinishedAuditAsync(FinishStatusAuditSystemRequest request)
    {
        var query = @$"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_actualiza_final_pista_auditoria(
                @p_id_pistas_auditoria
            )";
        using var connection = _context.CreateConnection();
        var result = await connection.GetFirstOrDefaultAsync<string, FinishStatusAuditSystemRequest>(query, request);
        return result ?? string.Empty;
    }

    public async Task<List<StageCatalogResponse>> GetStagesCatalogAsync()
    {
        var query = $@"
                    SELECT
                    *
                    FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.obtener_etapas();";

        using var connection = _context.CreateConnection();
        var results = await connection.QueryAsync<StageCatalogResponse>(query);
        return results?.ToList() ?? new List<StageCatalogResponse>();
    }
}