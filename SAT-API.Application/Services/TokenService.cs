using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SAT_API.Application.DTOs;
using SAT_API.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security;

namespace SAT_API.Application.Services;

public class TokenService: ITokenService
{
    private readonly ILogger<TokenService> _logger;

    public TokenService(ILogger<TokenService> logger)
    {
        _logger = logger;
    }

    public Task<SatUserInfoDto> ValidateAndExtractUserInfoAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            // Validar issuer del SAT
            if (!jsonToken.Issuer.Contains("authdev.mat.sat.gob.mx"))
            {
                throw new SecurityTokenInvalidIssuerException("Token no proviene del SAT");
            }

            // Extraer información del usuario
            var userInfo = MapTokenToUserInfo(jsonToken);

            _logger.LogInformation("Token válido para usuario: {UsuarioNombre} ({UsuarioRfc})",userInfo.NombreCompleto,userInfo.Rfc);
            return Task.FromResult(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validando token: {Message}", ex.Message);
            throw new SecurityException("Error al validar el token.", ex);
        }
    }

    public bool IsTokenValid(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.ValidTo >= DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    public DateTime GetTokenExpiration(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);
        return jsonToken.ValidTo;
    }

    private static SatUserInfoDto MapTokenToUserInfo(JwtSecurityToken jsonToken)
    {
        var userInfo = new SatUserInfoDto();

        foreach (var claim in jsonToken.Claims)
        {
            switch (claim.Type)
            {
                case "iss":
                    userInfo.Iss = claim.Value;
                    break;
                case "jti":
                    userInfo.Jti = claim.Value;
                    break;
                case "aud":
                    userInfo.Aud = claim.Value;
                    break;
                case "exp":
                    userInfo.Exp = long.Parse(claim.Value);
                    break;
                case "iat":
                    userInfo.Iat = long.Parse(claim.Value);
                    break;
                case "nbf":
                    userInfo.Nbf = long.Parse(claim.Value);
                    break;
                case "sub":
                    userInfo.Sub = claim.Value;
                    break;
                case "adminGral":
                    userInfo.AdminGral = claim.Value;
                    break;
                case "rfc_largo":
                    userInfo.RfcLargo = claim.Value;
                    break;
                case "roles":
                case "Roles":
                    if (claim.ValueType == "http://www.w3.org/2001/XMLSchema#string")
                    {
                        userInfo.RolesString = claim.Value;
                          // Si es un string, lo convertimos a lista
                        userInfo.Roles.AddRange(claim.Value.Split(',').Select(r => r.Trim()).ToList());
                    }
                    else
                    {
                        // Los roles vienen como array JSON, los parseamos
                        userInfo.Roles.AddRange(System.Text.Json.JsonSerializer.Deserialize<List<string>>(claim.Value) ?? new List<string>());
                    }
                    
                    break;
                case "cn":
                    userInfo.Cn = claim.Value;
                    break;
                case "nombreCompleto":
                    userInfo.NombreCompleto = claim.Value;
                    break;
                case "rfc":
                    userInfo.Rfc = claim.Value;
                    break;
                case "descEntFederativa":
                    userInfo.DescEntFederativa = claim.Value;
                    break;
                case "descAdminGral":
                    userInfo.DescAdminGral = claim.Value;
                    break;
                case "WorkforceID":
                    userInfo.WorkforceId = claim.Value;
                    break;
                case "descAdminCentral":
                    userInfo.DescAdminCentral = claim.Value;
                    break;
                case "adminCentral":
                    userInfo.AdminCentral = claim.Value;
                    break;
                case "email":
                    userInfo.Email = claim.Value;
                    break;
                case "curp":
                    userInfo.Curp = claim.Value;
                    break;
                case "scope":
                    if(claim.ValueType == "http://www.w3.org/2001/XMLSchema#string")
                    {
                        // Si es un string, lo convertimos a lista
                        userInfo.Scope = new List<string> { claim.Value };
                    }
                    else
                    {
                        // Si es un array JSON, lo parseamos
                        userInfo.Scope = System.Text.Json.JsonSerializer.Deserialize<List<string>>(claim.Value) ?? new List<string>();
                    }
                    break;
            }
        }

        return userInfo;
    }
}
