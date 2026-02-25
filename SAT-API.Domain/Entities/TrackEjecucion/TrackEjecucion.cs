namespace SAT_API.Domain.Entities;

public class TrackEjecucion
{
    public int TrackId { get; set; }
    public int EjecucionId { get; set; }
    public int EstatusEjecucionId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaModificacion { get; set; }
}
