using SAT_API.Application.Common;
using SAT_API.Application.DTOs.UserOperations;

namespace SAT_API.Application.Interfaces.UserOperations;

public interface IRoleManagementService
{
    // Define methods for role management here 
    Task<Result<RoleManagementReponseDto>> GetRoleByIdAsync(int roleId);
    Task<Result<IEnumerable<RoleManagementReponseDto>>> GetAllRolesAsync();
}
