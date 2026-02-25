namespace SAT_API.Domain.Entities.Products;

public class ControlNumbersRequest
{
    [ColumnMap("p_id_ejecucion")]
    public int ExecutionId { get; set; }
    [ColumnMap("p_id_etapa")]
    public int StageId { get; set; }
    [ColumnMap("p_id_archivo")]
    public int? FileId { get; set; }
}
