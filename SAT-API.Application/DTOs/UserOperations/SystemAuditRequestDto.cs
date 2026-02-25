namespace SAT_API.Application.DTOs.UserOperations;

public class SystemAuditRequestDto
{
    public int ExecutionId { get; set; }
    public string? Environment { get; set; }
    public string? User { get; set; }
    public string? MacAddressIp { get; set; }
    public int AuditTrailId { get; set; }
    public int Stage { get; set; }
    public int? FileId { get; set; }
}
