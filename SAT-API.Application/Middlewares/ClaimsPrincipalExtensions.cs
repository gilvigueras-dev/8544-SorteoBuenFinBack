using System.Security.Claims;

namespace SAT_API.Application
{
    /// <summary>
    /// Métodos de extensión para facilitar el acceso a los claims del token JWT del SAT
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        // ===== INFORMACIÓN BÁSICA =====
        
        /// <summary>
        /// Obtiene el ID único del usuario del token JWT del SAT
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene el nombre completo del usuario del token JWT del SAT
        /// </summary>
        public static string GetUserName(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene el email del usuario del token JWT del SAT
        /// </summary>
        public static string GetEmail(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        
        // ===== INFORMACIÓN ESPECÍFICA DEL SAT =====
        
        /// <summary>
        /// Obtiene el RFC del usuario del token JWT del SAT
        /// </summary>
        public static string GetRFC(this ClaimsPrincipal user)
            => user.FindFirst("rfc")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene el RFC largo del usuario del token JWT del SAT
        /// </summary>
        public static string GetRFCLargo(this ClaimsPrincipal user)
            => user.FindFirst("rfc_largo")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene el CN (Common Name) del usuario del token JWT del SAT
        /// </summary>
        public static string GetCN(this ClaimsPrincipal user)
            => user.FindFirst("cn")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene la CURP del usuario del token JWT del SAT
        /// </summary>
        public static string GetCURP(this ClaimsPrincipal user)
            => user.FindFirst("curp")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene el WorkforceID del usuario del token JWT del SAT
        /// </summary>
        public static string GetWorkforceId(this ClaimsPrincipal user)
            => user.FindFirst("workforceId")?.Value ?? string.Empty;
        
        // ===== INFORMACIÓN ORGANIZACIONAL DEL SAT =====
        
        /// <summary>
        /// Obtiene el código de Administración General del usuario del token JWT del SAT
        /// </summary>
        public static string GetAdminGeneral(this ClaimsPrincipal user)
            => user.FindFirst("adminGral")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene el código de Administración Central del usuario del token JWT del SAT
        /// </summary>
        public static string GetAdminCentral(this ClaimsPrincipal user)
            => user.FindFirst("adminCentral")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene la descripción de Administración General del usuario del token JWT del SAT
        /// </summary>
        public static string GetDescripcionAdminGeneral(this ClaimsPrincipal user)
            => user.FindFirst("descAdminGral")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene la descripción de Administración Central del usuario del token JWT del SAT
        /// </summary>
        public static string GetDescripcionAdminCentral(this ClaimsPrincipal user)
            => user.FindFirst("descAdminCentral")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene la entidad federativa del usuario del token JWT del SAT
        /// </summary>
        public static string GetEntidadFederativa(this ClaimsPrincipal user)
            => user.FindFirst("descEntFederativa")?.Value ?? string.Empty;
        
        // ===== ROLES Y PERMISOS =====
        
        /// <summary>
        /// Obtiene todos los roles del usuario del token JWT del SAT
        /// </summary>
        public static List<string> GetRoles(this ClaimsPrincipal user)
            => user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        
        /// <summary>
        /// Obtiene todos los scopes del usuario del token JWT del SAT
        /// </summary>
        public static List<string> GetScopes(this ClaimsPrincipal user)
            => user.FindAll("scope").Select(c => c.Value).ToList();
        
        /// <summary>
        /// Obtiene los roles completos originales del SAT (estructura completa con cn=)
        /// </summary>
        public static List<string> GetFullRoles(this ClaimsPrincipal user)
            => user.FindAll("sat_role_full").Select(c => c.Value).ToList();
        
        // ===== VERIFICACIONES RÁPIDAS =====
        
        /// <summary>
        /// Verifica si el usuario pertenece a la Administración General de Recaudación (AGR)
        /// </summary>
        public static bool IsFromAGR(this ClaimsPrincipal user)
            => user.GetAdminGeneral() == "400";
        
        /// <summary>
        /// Verifica si el usuario tiene un rol específico
        /// </summary>
        public static bool HasRole(this ClaimsPrincipal user, string role)
            => user.HasClaim(ClaimTypes.Role, role);
        
        /// <summary>
        /// Verifica si el usuario tiene alguno de los roles especificados
        /// </summary>
        public static bool HasAnyRole(this ClaimsPrincipal user, params string[] roles)
            => roles.Any(role => user.HasClaim(ClaimTypes.Role, role));
        
        /// <summary>
        /// Verifica si el usuario tiene algún rol que contenga el texto especificado
        /// </summary>
        public static bool HasRoleContaining(this ClaimsPrincipal user, string rolePart)
            => user.GetRoles().Any(r => r.Contains(rolePart, StringComparison.OrdinalIgnoreCase));
        
        /// <summary>
        /// Verifica si el usuario tiene un scope específico
        /// </summary>
        public static bool HasScope(this ClaimsPrincipal user, string scope)
            => user.HasClaim("scope", scope);
        
        /// <summary>
        /// Verifica si el usuario tiene email válido
        /// </summary>
        public static bool HasValidEmail(this ClaimsPrincipal user)
        {
            var email = user.GetEmail();
            return !string.IsNullOrEmpty(email) && email.Contains('@') && email.Contains('.');
        }
        
        /// <summary>
        /// Verifica si el usuario tiene CURP válida (no es "X")
        /// </summary>
        public static bool HasValidCURP(this ClaimsPrincipal user)
        {
            var curp = user.GetCURP();
            return !string.IsNullOrEmpty(curp) && curp != "X";
        }
        
        // ===== VERIFICACIONES DE ROLES ESPECÍFICOS DEL SAT =====
        
        /// <summary>
        /// Verifica si el usuario tiene algún rol de CRM
        /// </summary>
        public static bool IsCRMUser(this ClaimsPrincipal user)
            => user.HasRoleContaining("CRM");
        
        /// <summary>
        /// Verifica si el usuario tiene algún rol de SIATSS
        /// </summary>
        public static bool IsSIATSSUser(this ClaimsPrincipal user)
            => user.HasRoleContaining("SIATSS");
        
        /// <summary>
        /// Verifica si el usuario tiene algún rol de administrador
        /// </summary>
        public static bool IsAdminUser(this ClaimsPrincipal user)
            => user.HasRoleContaining("ADMIN");
        
        /// <summary>
        /// Verifica si el usuario tiene acceso a PeopleSoft
        /// </summary>
        public static bool HasPeopleSoftAccess(this ClaimsPrincipal user)
            => user.HasRoleContaining("PEOPLESOFT");
        
        /// <summary>
        /// Verifica si el usuario puede certificar documentos
        /// </summary>
        public static bool CanCertifyDocuments(this ClaimsPrincipal user)
            => user.HasRoleContaining("CERTIF");
        
        /// <summary>
        /// Verifica si el usuario es analista (puede ver reportes)
        /// </summary>
        public static bool IsAnalyst(this ClaimsPrincipal user)
            => user.HasRoleContaining("ANALISTA");
        
        // ===== INFORMACIÓN DE TOKEN =====
        
        /// <summary>
        /// Obtiene la fecha de expiración del token
        /// </summary>
        public static DateTime? GetTokenExpiration(this ClaimsPrincipal user)
        {
            var exp = user.FindFirst("exp")?.Value;
            if (long.TryParse(exp, out long expValue))
            {
                return DateTimeOffset.FromUnixTimeSeconds(expValue).DateTime;
            }
            return null;
        }
        
        /// <summary>
        /// Verifica si el token está expirado
        /// </summary>
        public static bool IsTokenExpired(this ClaimsPrincipal user)
        {
            var expiration = user.GetTokenExpiration();
            return expiration.HasValue && expiration.Value < DateTime.UtcNow;
        }
        
        /// <summary>
        /// Obtiene el issuer (emisor) del token
        /// </summary>
        public static string GetTokenIssuer(this ClaimsPrincipal user)
            => user.FindFirst("iss")?.Value ?? string.Empty;
        
        /// <summary>
        /// Obtiene el ID único del token (jti)
        /// </summary>
        public static string GetTokenId(this ClaimsPrincipal user)
            => user.FindFirst("jti")?.Value ?? string.Empty;
        
        // ===== RESUMEN COMPLETO DEL USUARIO =====
        
        /// <summary>
        /// Obtiene un resumen completo del usuario para logging o debugging
        /// </summary>
        public static object GetUserSummary(this ClaimsPrincipal user)
        {
            return new
            {
                UserId = user.GetUserId(),
                UserName = user.GetUserName(),
                RFC = user.GetRFC(),
                Email = user.GetEmail(),
                WorkforceId = user.GetWorkforceId(),
                AdminGeneral = user.GetAdminGeneral(),
                EntidadFederativa = user.GetEntidadFederativa(),
                IsFromAGR = user.IsFromAGR(),
                IsCRMUser = user.IsCRMUser(),
                IsSIATSSUser = user.IsSIATSSUser(),
                IsAdminUser = user.IsAdminUser(),
                RoleCount = user.GetRoles().Count,
                ScopeCount = user.GetScopes().Count,
                HasValidEmail = user.HasValidEmail(),
                TokenExpiration = user.GetTokenExpiration(),
                IsTokenExpired = user.IsTokenExpired()
            };
        }
    }
}
