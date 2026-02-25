namespace SAT_API.Domain.Entities.Databricks
{
    public class ClustersResponse
    {
        public List<DatabricksCluster> clusters { get; set; } = new();
    }
}
