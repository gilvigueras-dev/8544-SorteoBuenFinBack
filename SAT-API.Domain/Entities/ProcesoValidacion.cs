
namespace SAT_API.Domain.Entities;
public class ProcesoValidacion
{
    public string ProcesoId { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string RunId { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
