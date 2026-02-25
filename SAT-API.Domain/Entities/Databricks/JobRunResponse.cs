namespace SAT_API.Domain.Entities.Databricks
{
    public class JobRunsResponse
    {
        public List<JobRun> runs { get; set; } = new();
        public bool has_more { get; set; }
    }
}
