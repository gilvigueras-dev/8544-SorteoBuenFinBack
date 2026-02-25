using Dapper;
using SAT_API.Domain.Entities;
using SAT_API.Domain.Interfaces;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories;

public class JobConfigRepository : IJobConfigRepository
{
    private readonly IDbContext _context;

    public JobConfigRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<JobConfig?> GetByNombreAsync(string nombre)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_job_config(@nombre) ";
        using var connection = _context.CreateConnection();
        List<JobConfig> jobConfigs = (await connection.QueryAsync<JobConfig>(query, new { nombre = nombre })).ToList();
        return jobConfigs.FirstOrDefault();
    }
}
