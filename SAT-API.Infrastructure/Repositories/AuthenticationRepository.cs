using Dapper;
using SAT_API.Domain.Entities.Authentication;
using SAT_API.Domain.Interfaces;
using SAT_API.Infrastructure.Data;

namespace SAT_API.Infrastructure.Repositories;

public class AuthenticationRepository: IAuthenticationRepository
{
     private readonly IDbContext _context;
    public AuthenticationRepository(IDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserRoleResponse?> GetUserRoleAsync(string dni)
    {
        var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_rol(@p_rfc)";

        using var connection = _context.CreateConnection();
        var result = await connection.QueryFirstOrDefaultAsync<UserRoleResponse?>(query, new { p_rfc = dni });

        return result;
    }
}
