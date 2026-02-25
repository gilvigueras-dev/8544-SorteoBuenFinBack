namespace SAT_API.Domain.Entities;

public class JobConfig
{
    public int JobConfigId { get; set; } 
    public string Nombre { get; set; } = string.Empty;
    public string JobId { get; set; } = string.Empty;
    public string RunId { get; set; } = string.Empty;
}
