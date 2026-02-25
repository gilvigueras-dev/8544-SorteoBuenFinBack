using System.Text.Json.Serialization;

namespace SAT_API.Domain.Entities.Databricks
{
    public class JobResponse
    {
        [JsonPropertyName("created_time")]
        public long? CreatedTime { get; set; }
        [JsonPropertyName("creator_user_name")]
        public string CreatorUserName { get; set; } = string.Empty;
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
        [JsonPropertyName("job_id")]
        public long JobId { get; set; }
        [JsonPropertyName("run_as_user_name")]
        public string RunAsUserName { get; set; } = string.Empty;
        [JsonPropertyName("settings")]
        public JobSettings Settings { get; set; } = new ();
        [JsonPropertyName("timeout_seconds")]
        public int TimeoutsSeconds { get; set; }
    }

    public class JobSettings { 
        [JsonPropertyName("description")]
        public string Descripcion { get; set; }= string.Empty;
        [JsonPropertyName("edit_mode")]
        public string EditMode { get; set; } = string.Empty;
        [JsonPropertyName("format")]
        public string Format { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
