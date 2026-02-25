using System.Security.Claims;
using SAT_API.Application.DTOs;

namespace SAT_API.Application.Interfaces;

public interface IClaimService
{
        ClaimsIdentity CreateClaimsIdentity(SatUserInfoDto userInfo);
        List<string> ExtractRolesFromToken(SatUserInfoDto userInfo);
        bool UserHasRole(SatUserInfoDto userInfo, string role);
        bool UserHasAnyRole(SatUserInfoDto userInfo, params string[] roles);
        ClaimsPrincipal GetCurrentUser();
}
