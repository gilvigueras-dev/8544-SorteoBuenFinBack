

namespace SAT_API.Application.DTOs.Ejecuciones;

public class TrackEjecucionDto
{
    public int EjecucionId { get; set; }
    public int EstatusEjecucionId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaModificacion { get; set; }
}
