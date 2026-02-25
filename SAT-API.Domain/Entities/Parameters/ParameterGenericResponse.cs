namespace SAT_API.Domain.Entities.Parameters;

public class ParameterGenericResponse
{
    [ColumnMap("id")]
    public int ParameterId { get; set; }
    [ColumnMap("tipo")]
    public string Type { get; set; } = string.Empty;
    [ColumnMap("valor")]
    public string Value { get; set; } = string.Empty;
}
