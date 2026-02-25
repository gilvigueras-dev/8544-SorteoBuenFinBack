using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAT_API.Application.Interfaces;

namespace SAT_API.Application;

public static class TokenConfig
{
    public static BearerTokenEvents ConfigureBearerTokenEvents()
    {
        return new BearerTokenEvents
        {
            OnMessageReceived = async context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetService<ILoggerFactory>()?.CreateLogger("TokenConfig");

                logger?.LogInformation("Iniciando procesamiento de token JWT del SAT");
                logger?.LogDebug("Path: {Path}, Method: {Method}", context.Request.Path, context.Request.Method);

                var token = ExtractTokenFromRequest(context, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);

                if (string.IsNullOrEmpty(token))
                {
                    logger?.LogWarning("No se encontró token válido en el request");
                    context.NoResult(); // Usar NoResult() en lugar de throw
                    return;
                }

                logger?.LogInformation("Token JWT del SAT encontrado");

                if (await TryProcessWithApplicationServices(context, token, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance))
                {
                    return;
                }

                await ProcessTokenDirectly(context, token, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            }
        };
    }

    private static async Task ProcessTokenDirectly(MessageReceivedContext context, string token, ILogger logger)
    {
        logger?.LogInformation("Procesando token directamente");

        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(token))
        {
            logger?.LogError("Token JWT inválido o malformado");
            context.Fail("Token JWT con formato inválido");
            return;
        }

        var jsonToken = tokenHandler.ReadJwtToken(token);
        ///logger?.LogInformation("Token JWT leído. Issuer: {Issuer}, Exp: {Expiration}", jsonToken.Issuer, jsonToken.ValidTo);

        if (jsonToken.ValidTo < DateTime.UtcNow)
        {
            logger?.LogWarning("Token expirado. Expiró: {Expiration}", jsonToken.ValidTo);
            context.Fail($"Token JWT expirado el {jsonToken.ValidTo:yyyy-MM-dd HH:mm:ss} UTC");
            return;
        }

        var claims = ProcessTokenClaims(jsonToken, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);

        if (claims == null || claims.Count > 0)
        {
            logger?.LogError("No se pudieron procesar los claims del token JWT");
            context.Fail("No se pudieron procesar los claims del token JWT");
            return;
        }

        ///logger?.LogInformation("Claims procesados directamente: {ClaimsCount}", claims.Count);

        var directIdentity = new ClaimsIdentity(claims, "SatTokenDirect");
        context.Principal = new ClaimsPrincipal(directIdentity);
        context.HttpContext.Items["AuthenticationMethod"] = "DirectProcessing";

        LogUserInformation(claims, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
        logger?.LogInformation("Autenticación completada con procesamiento directo");

        context.Success();
        await Task.CompletedTask;
    }

    private static async Task<bool> TryProcessWithApplicationServices(MessageReceivedContext context, string token, ILogger logger)
    {
        logger?.LogDebug("Intentando usar servicios ITokenService e IClaimService");

        var tokenService = context.HttpContext.RequestServices.GetService<ITokenService>();
        var claimsService = context.HttpContext.RequestServices.GetService<IClaimService>();

        if (tokenService == null || claimsService == null)
        {
            logger?.LogWarning("Servicios ITokenService o IClaimService no disponibles");
            return false;
        }

        logger?.LogDebug("Servicios encontrados, procesando con capa Application");

        try
        {
            var userInfo = await tokenService.ValidateAndExtractUserInfoAsync(token);
            if (userInfo == null)
            {
                logger?.LogWarning("No se pudo extraer información de usuario del token");
                context.Fail("Token JWT inválido o información de usuario no disponible");
                return true; // Retornamos true porque manejamos el error
            }

            logger?.LogInformation("Token validado por TokenService para: {User}", userInfo.NombreCompleto);

            var identity = claimsService.CreateClaimsIdentity(userInfo);
            ///logger?.LogInformation("ClaimsIdentity creada con {ClaimsCount} claims", identity.Claims.Count());

            context.HttpContext.Items["SatUserInfo"] = userInfo;
            context.HttpContext.Items["AuthenticationMethod"] = "ApplicationServices";
            context.Principal = new ClaimsPrincipal(identity);
            context.Success();

            logger?.LogInformation("Autenticación completada con servicios Application");
            return true;
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Error en servicios Application, continuando con procesamiento directo");
            return false; // Permitir fallback a procesamiento directo
        }
    }

    private static string ExtractTokenFromRequest(MessageReceivedContext context, ILogger logger)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        logger?.LogDebug("Authorization header presente: {HasHeader}", !string.IsNullOrEmpty(authHeader));

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            logger?.LogDebug("Token extraído del header, longitud: {Length}", token.Length);
            return token;
        }

        var contextToken = context.Token ?? string.Empty;
        logger?.LogDebug("Token desde context: {HasToken}", !string.IsNullOrEmpty(contextToken));
        return contextToken;
    }

    private static void LogUserInformation(IList<Claim> claims, ILogger logger)
    {
        var userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Usuario desconocido";
        var userRfc = claims.FirstOrDefault(c => c.Type == "rfc")?.Value ?? "RFC desconocido";
        var roleCount = claims.Count(c => c.Type == ClaimTypes.Role);

        logger?.LogInformation("Usuario: {User}, RFC: {RFC}, Roles: {RoleCount}", userName, userRfc, roleCount);
    }

    // Resto de métodos sin cambios...
    private static List<Claim> ProcessTokenClaims(JwtSecurityToken jsonToken, ILogger logger)
    {
        var claims = new List<Claim>();

        foreach (var claim in jsonToken.Claims)
        {
            try
            {
                switch (claim.Type)
                {
                    case "sub":
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, claim.Value ?? string.Empty));
                        break;
                    case "nombreCompleto":
                        claims.Add(new Claim(ClaimTypes.Name, claim.Value ?? string.Empty));
                        break;
                    case "email":
                        claims.Add(new Claim(ClaimTypes.Email, claim.Value ?? string.Empty));
                        break;
                    case "rfc":
                        claims.Add(new Claim("rfc", claim.Value ?? string.Empty));
                        break;
                    case "rfc_largo":
                        claims.Add(new Claim("rfc_largo", claim.Value ?? string.Empty));
                        break;
                    case "adminGral":
                        claims.Add(new Claim("adminGral", claim.Value ?? string.Empty));
                        break;
                    case "adminCentral":
                        claims.Add(new Claim("adminCentral", claim.Value ?? string.Empty));
                        break;
                    case "cn":
                        claims.Add(new Claim("cn", claim.Value ?? string.Empty));
                        break;
                    case "WorkforceID":
                        claims.Add(new Claim("workforceId", claim.Value ?? string.Empty));
                        break;
                    case "descAdminGral":
                        claims.Add(new Claim("descAdminGral", claim.Value ?? string.Empty));
                        break;
                    case "descAdminCentral":
                        claims.Add(new Claim("descAdminCentral", claim.Value ?? string.Empty));
                        break;
                    case "descEntFederativa":
                        claims.Add(new Claim("descEntFederativa", claim.Value ?? string.Empty));
                        break;
                    case "curp":
                        claims.Add(new Claim("curp", claim.Value ?? string.Empty));
                        break;
                    case "Roles":
                    case "roles":
                        if (!string.IsNullOrEmpty(claim.Value))
                        {
                            ProcessRolesString(claim.Value, claims, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
                        }
                        break;
                    case "scope":
                        if (!string.IsNullOrEmpty(claim.Value))
                        {
                            ProcessScope(claim.Value, claims, logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
                        }
                        break;
                    default:
                        claims.Add(new Claim(claim.Type, claim.Value ?? string.Empty));
                        break;
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error procesando claim {Type}: {Value}",
                    claim.Type, claim.Value?.Substring(0, Math.Min(100, claim.Value?.Length ?? 0)));
            }
        }

        return claims;
    }

    private static void ProcessRolesString(string rolesString, List<Claim> claims, ILogger logger)
    {
        try
        {
            var stringRoles = rolesString.Split(',');
            logger?.LogDebug("Procesando {RoleCount} roles del string", stringRoles.Length);

            foreach (var role in stringRoles)
            {
                var trimmedRole = role.Trim();
                if (trimmedRole.StartsWith("cn="))
                {
                    var roleName = trimmedRole.Substring(3);
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }
                else if (!string.IsNullOrEmpty(trimmedRole))
                {
                    claims.Add(new Claim(ClaimTypes.Role, trimmedRole));
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Error procesando roles string: {Roles}", rolesString);
        }
    }

    private static void ProcessScope(string scopeValue, List<Claim> claims, ILogger logger)
    {
        try
        {
            var scopes = JsonSerializer.Deserialize<List<string>>(scopeValue) ?? new List<string>();
            foreach (var scope in scopes)
            {
                claims.Add(new Claim("scope", scope));
            }
            logger?.LogDebug("Scopes JSON procesados: {ScopeCount}", scopes.Count);
        }
        catch(Exception ex)
        {
            claims.Add(new Claim("scope", scopeValue));
            logger?.LogDebug(ex, "Scope string procesado: {Scope}", scopeValue);
            throw new InvalidOperationException("Error en el proceso scope", ex);
        }
    }
}