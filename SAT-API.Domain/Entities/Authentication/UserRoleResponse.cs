namespace SAT_API.Domain.Entities.Authentication;

public class UserRoleResponse
{
    [ColumnMap("rfc")]
    public string Dni { get; set; } = string.Empty;
    [ColumnMap("rol")]
    public string Role { get; set; } = string.Empty;
    [ColumnMap("rol_origen")]
    public string SourceRole { get; set; } = string.Empty;
}
