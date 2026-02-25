using SAT_API.Application.DTOs;

namespace SAT_API.Application.Interfaces;

public interface ITokenService
{
   Task<SatUserInfoDto> ValidateAndExtractUserInfoAsync(string token);
   bool IsTokenValid(string token);
   DateTime GetTokenExpiration(string token);
}
