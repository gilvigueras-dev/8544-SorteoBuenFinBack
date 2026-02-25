namespace SAT_API.Domain.Entities.Databricks
{
    public class DatabricksCluster
    {
        public string cluster_id { get; set; } = string.Empty;
        public string cluster_name { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string node_type_id { get; set; } = string.Empty;
        public int num_workers { get; set; }
    }
}
