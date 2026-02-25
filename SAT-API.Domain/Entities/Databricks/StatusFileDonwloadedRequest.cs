namespace SAT_API.Domain.Entities.Databricks;

public class StatusFileDownloadedRequest
{
    [ColumnMap("p_id_ejecucion")]
    public int ExecutionId { get; set; }
    [ColumnMap("p_id_etapa")]
    public int StageId { get; set; }
}
