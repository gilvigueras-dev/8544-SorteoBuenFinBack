using System.Text.Json.Serialization;

namespace SAT_API.Domain.Entities.Databricks
{
    public class JobExecuteRunResponse
    {
        [JsonPropertyName("run_id")]
        public decimal RunId { get; set; }

        [JsonPropertyName("number_in_job")]
        public long NumberInJob { get; set; }
    }
}
