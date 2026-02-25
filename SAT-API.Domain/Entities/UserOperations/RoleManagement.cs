namespace SAT_API.Domain.Entities.UserOperations;

public class RoleManagement
{
    [ColumnMap("id_rol")]
    public int RoleId { get; set; }
    [ColumnMap("rol_origen")]
    public string RoleOrigin { get; set; } = string.Empty;
    [ColumnMap("rol")]
    public string RoleName { get; set; } = string.Empty;
    [ColumnMap("descripcion")]
    public string RoleDescription { get; set; } = string.Empty;
    [ColumnMap("rol_tecnico")]
    public string RoleTechnician { get; set; } = string.Empty;
}
