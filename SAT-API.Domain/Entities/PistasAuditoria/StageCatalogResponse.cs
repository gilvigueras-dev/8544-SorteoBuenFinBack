namespace SAT_API.Domain.Entities.PistasAuditoria;

public class StageCatalogResponse
{
    [ColumnMap("id_etapa")]
    public int StageId { get; set; }
    [ColumnMap("etapa")]
    public string Stage { get; set; } = string.Empty;
}
