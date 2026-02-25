namespace SAT_API.Domain.Entities;

public class PistaAuditoria
{
    public int Id { get; set; }
    public int EjecucionId { get; set; }
    public string? Actividad { get; set; }
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }
}
