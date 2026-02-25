using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SAT_API.Application.DTOs;
using SAT_API.Application.Interfaces;

namespace SAT_API.Application.Services;

public class ClaimsService : IClaimService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            throw new InvalidOperationException("No current user found in HttpContext.");
        }
        return user;
    }

    public ClaimsIdentity CreateClaimsIdentity(SatUserInfoDto userInfo)
    {
        var claims = new List<Claim>
            {
                // Claims estándar
                new Claim(ClaimTypes.NameIdentifier, userInfo.Sub),
                new Claim(ClaimTypes.Name, userInfo.NombreCompleto),
                new Claim(ClaimTypes.Email, userInfo.Email ?? ""),
                new Claim("rfc", userInfo.Rfc),
                new Claim("rfc_largo", userInfo.RfcLargo ?? ""),
                new Claim("cn", userInfo.Cn ?? ""),
                new Claim("curp", userInfo.Curp ?? ""),
                
                // Claims administrativos del SAT
                new Claim("adminGral", userInfo.AdminGral ?? ""),
                new Claim("adminCentral", userInfo.AdminCentral ?? ""),
                new Claim("descAdminGral", userInfo.DescAdminGral ?? ""),
                new Claim("descAdminCentral", userInfo.DescAdminCentral ?? ""),
                new Claim("descEntFederativa", userInfo.DescEntFederativa ?? ""),
                new Claim("workforceId", userInfo.WorkforceId ?? ""),
                
                // Claims del token
                new Claim("iss", userInfo.Iss),
                new Claim("jti", userInfo.Jti),
                new Claim("aud", userInfo.Aud)
            };

        // Agregar roles simplificados del SAT
        var simplifiedRoles = ExtractRolesFromToken(userInfo);
        foreach (var role in simplifiedRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Agregar roles completos como claims personalizados
        foreach (var fullRole in userInfo.Roles)
        {
            claims.Add(new Claim("sat_role_full", fullRole));
        }

        // Agregar scopes
        foreach (var scope in userInfo.Scope)
        {
            claims.Add(new Claim("scope", scope));
        }

        return new ClaimsIdentity(claims, "SatToken");
    }

    public List<string> ExtractRolesFromToken(SatUserInfoDto userInfo)
    {
        var roles = new HashSet<string>();

        foreach (var fullRole in userInfo.Roles)
        {
            // Extraer el nombre del rol de la estructura compleja del SAT
            // Ejemplo: "cn=SAT_CRM_DP_ADMIN_LOCAL_RECAUD,cn=CRM,cn=Level10..." -> "SAT_CRM_DP_ADMIN_LOCAL_RECAUD"
            var parts = fullRole.Split(',');
            if (parts.Length > 0 && parts[0].StartsWith("cn="))
            {
                var roleName = parts[0].Substring(3); // Quitar "cn="
                roles.Add(roleName);
            }
        }

        // Agregar roles desde el string de roles si existe
        if (!string.IsNullOrEmpty(userInfo.RolesString))
        {
            var  rol = userInfo.RolesString
                .Split(',')
                .Where(role => role.StartsWith("cn="))
                .Select(role => role.Substring(3))
                .ToList();

            return rol;
        }

        return new List<string>(); // Retornar una lista vacía si no hay roles

    }

    public bool UserHasRole(SatUserInfoDto userInfo, string role)
    {
        var userRoles = ExtractRolesFromToken(userInfo);
        return userRoles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    public bool UserHasAnyRole(SatUserInfoDto userInfo, params string[] roles)
    {
        var userRoles = ExtractRolesFromToken(userInfo);
        return roles.Any(role => userRoles.Any(ur => ur.Equals(role, StringComparison.OrdinalIgnoreCase)));
    }
}
