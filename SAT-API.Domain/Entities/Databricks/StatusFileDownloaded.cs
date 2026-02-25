
namespace SAT_API.Domain.Entities.Databricks;

public class StatusFileDownloadedResponse
{
    [ColumnMap("estado")]
    public string Status { get; set; } = string.Empty;
}
