using Dapper;
using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Parameters;
using SAT_API.Domain.Entities.Parametros;
using SAT_API.Domain.Interfaces.Parameters;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories.Parameters;

public class ParametersRepository : IParametersRepository
{
    private readonly IDbContext _context;

    public ParametersRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<string> SetParametros(ParameterInsertRequest request)
    {
        var query = $"SELECT {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_insertavaloresparametros(@p_id_ejecucion, {request.Parameters})";

        using var connection = _context.CreateConnection();

        var result = await connection.QuerySingleAsync<string>(query, new { p_id_ejecucion = request.ExecutionId });
        return result;
    }

    public async Task<List<ParameterResponse>> ObtenerParametros(int ejecucionId)
    {
        var query = $"select * from {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_parametros(@ejecucionId)";

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<ParameterResponse>(query, new { ejecucionId = ejecucionId });
        return result.ToList();
    }

    public async Task<List<ParameterGenericResponse>> AllActiveParameters()
    { 
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_parametros_activos()";

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<ParameterGenericResponse>(query);
        return result.ToList();
    }
}
