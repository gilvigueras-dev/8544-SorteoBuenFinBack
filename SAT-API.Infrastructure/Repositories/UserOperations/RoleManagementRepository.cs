using Dapper;
using SAT_API.Domain.Entities.UserOperations;
using SAT_API.Domain.Interfaces.UserOperations;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories.UserOperations;

public class RoleManagementRepository : IRoleManagementRepository
{   
    private readonly IDbContext _context;
    public RoleManagementRepository(IDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<RoleManagement>> GetAllRolesAsync()
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_roles()";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<RoleManagement>(query);
    }

    public async Task<RoleManagement?> GetRoleByIdAsync(int roleId)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_roles(@p_id_rol)";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<RoleManagement>(query, new { p_id_rol = roleId });
    }
}
