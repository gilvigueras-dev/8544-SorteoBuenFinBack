namespace SAT_API.Domain.Entities.Databricks
{
    public class DatabricksConnectionSettings
    {
        public string InstanceUrl { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string DefaultClusterId { get; set; } = string.Empty;
        public int RequestTimeoutSeconds { get; set; }
    }
}
