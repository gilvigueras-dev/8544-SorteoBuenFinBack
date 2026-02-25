using SAT_API.Domain.Entities.Authentication;

namespace SAT_API.Domain.Interfaces;

public interface IAuthenticationRepository
{
    Task<UserRoleResponse?> GetUserRoleAsync(string dni);
}
