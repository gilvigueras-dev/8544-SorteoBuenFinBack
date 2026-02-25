namespace SAT_API.Application.DTOs;

public class SatUserInfoDto
{
    public string Iss { get; set; } = string.Empty;
    public string Jti { get; set; } = string.Empty;
    public string Aud { get; set; } = string.Empty;
    public long Exp { get; set; }
    public long Iat { get; set; }
    public long Nbf { get; set; }
    public string Sub { get; set; } = string.Empty;

    public string AdminGral { get; set; } = string.Empty;
    public string RfcLargo { get; set; } = string.Empty;
    public string RolesString { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string Cn { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rfc { get; set; } = string.Empty;
    public string DescEntFederativa { get; set; } = string.Empty;
    public string DescAdminGral { get; set; } = string.Empty;
    public string WorkforceId { get; set; } = string.Empty;
    public string DescAdminCentral { get; set; } = string.Empty;
    public string AdminCentral { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Curp { get; set; } = string.Empty;
    public List<string> Scope { get; set; } = new();

    public DateTime ExpirationDate => DateTimeOffset.FromUnixTimeSeconds(Exp).DateTime;
    public DateTime IssuedAtDate => DateTimeOffset.FromUnixTimeSeconds(Iat).DateTime;
    public bool IsExpired => DateTime.UtcNow > ExpirationDate;
}
