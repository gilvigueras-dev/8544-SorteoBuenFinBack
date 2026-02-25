

namespace SAT_API.Application.DTOs.Ejecuciones;

public class EjecucionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int IdEstatusEjecucion { get; set; }
    public string EstatusEjecucion { get; set; } = string.Empty;
    public string Estatus { get; set; } = string.Empty;
}
