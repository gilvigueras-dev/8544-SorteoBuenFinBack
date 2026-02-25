namespace SAT_API.Domain.Entities.Products
{
    public class AuditTrailResponse
    {
        public int EventId { get; set; }
        public string Folio { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public int Stage { get; set; }
        public int ExecutionId { get; set; }
        public DateTime? StartQueryDate { get; set; }
        public DateTime? EndQueryDate { get; set; }
        public string Action { get; set; } = string.Empty;
        public string FileRole { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string IPOrIdentifier { get; set; } = string.Empty;
    }
}