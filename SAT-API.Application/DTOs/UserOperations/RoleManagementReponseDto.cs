namespace SAT_API.Application.DTOs.UserOperations;

public class RoleManagementReponseDto
{
    public int IdRol { get; set; }
    public string RolOrigen { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string RolTecnico { get; set; } = string.Empty;
}
