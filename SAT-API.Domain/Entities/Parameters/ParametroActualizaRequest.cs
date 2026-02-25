namespace SAT_API.Domain.Entities.Parametros;

public class ParameterInsertRequest
{
    [ColumnMap("p_id_ejecucion")]
    public int ExecutionId { get; set; }
    [ColumnMap("entradas")]
    public string Parameters { get; set; } = string.Empty;
}
