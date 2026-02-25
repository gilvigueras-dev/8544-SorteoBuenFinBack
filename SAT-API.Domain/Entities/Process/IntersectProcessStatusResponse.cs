namespace SAT_API.Domain.Entities.Process;

public class IntersectProcessStatusResponse
{
    [ColumnMap("estatus_valor")]
    public int Value { get; set; }
    [ColumnMap("estatus")]
    public string Status { get; set; } = string.Empty;
}
