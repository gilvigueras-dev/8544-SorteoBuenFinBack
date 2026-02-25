namespace SAT_API.Domain.Entities;

public class ParameterResponse
{
    [ColumnMap("id_parametros")]
    public int ParameterId { get; set; }
    [ColumnMap("valor")]
    public string Value { get; set; } = string.Empty;
}
