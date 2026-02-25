namespace SAT_API.Domain.Entities.UserOperations;

public class SystemAuditRequest
{
    [ColumnMap("p_id_ejecucion")]
    public int ExecutionId { get; set; }
    [ColumnMap("p_ambiente")]
    public string? Environment { get; set; }
    [ColumnMap("p_usuario")]
    public string? User { get; set; }
    [ColumnMap("p_mac_address_ip")]
    public string? MacAddressIp { get; set; }
    [ColumnMap("p_id_pista_auditoria")]
    public int AuditTrailId { get; set; }
    [ColumnMap("p_etapa")]
    public int Stage { get; set; }
    [ColumnMap("p_id_archivo")]
    public int? FileId { get; set; }
}
