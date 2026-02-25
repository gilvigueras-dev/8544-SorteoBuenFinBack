using SAT_API.Domain.Entities.UserOperations;

namespace SAT_API.Domain.Interfaces.UserOperations;

public interface IRoleManagementRepository
{
    Task<RoleManagement?> GetRoleByIdAsync(int roleId);
    Task<IEnumerable<RoleManagement>> GetAllRolesAsync();

}
