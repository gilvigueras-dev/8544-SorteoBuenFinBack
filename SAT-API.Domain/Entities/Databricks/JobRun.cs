using System.Text.Json.Serialization;

namespace SAT_API.Domain.Entities.Databricks
{
    public class JobRun
    {
        [JsonPropertyName("run_id")]
        public long RunId { get; set; }
        [JsonPropertyName("run_name")]
        public string RunName { get; set; } = string.Empty;
        [JsonPropertyName("state")]
        public JobState State { get; set; } = new();
        [JsonPropertyName("start_time")]
        public long? StartTime { get; set; }
        [JsonPropertyName("end_time")]
        public long? EndTime { get; set; }
        public DateTime? StartDateTime => StartTime is long ticks ? new DateTime(ticks, DateTimeKind.Utc): null;
        public DateTime? EndDateTime => EndTime is long ticks ? new DateTime(ticks, DateTimeKind.Utc) : null;
    }

    public class JobState {
        [JsonPropertyName("life_cicle_state")]
        public string LifeCicleState { get; set; } = string.Empty;
        [JsonPropertyName("result_state")]
        public string ResultState { get; set; } = string.Empty;
        [JsonPropertyName("state_message")]
        public string StateMessage { get; set; } = string.Empty;
        [JsonPropertyName("user_cancelled_or_timedout")]
        public bool UserCancelledOrTimedout { get; set; }
    }
}
