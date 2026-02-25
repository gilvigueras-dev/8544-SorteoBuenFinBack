namespace SAT_API.Domain.Entities.PistasAuditoria;

public class FinishStatusAuditSystemRequest
{
    [ColumnMap("p_id_pistas_auditoria")]
    public int AuditSystemId { get; set; }
}
