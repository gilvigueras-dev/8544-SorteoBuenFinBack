namespace SAT_API.Domain.Entities.Databricks;

public class DbfsFilePathResponse
{
    [ColumnMap("ruta_informe")]
    public string UrlPath { get; set; } = string.Empty;
}

 public class DbfsReadResponse
    {
        public long bytes_read { get; set; }
        public string data { get; set; } = string.Empty;
    }
