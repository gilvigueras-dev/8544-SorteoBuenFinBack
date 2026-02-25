namespace SAT_API.Application.DTOs.Insumos;

public record JobConfigDto
{
    public int JobConfigId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string JobId { get; set; } = string.Empty;
    public string RunId { get; set; } = string.Empty;
}
