using SAT_API.Domain.Enums;

namespace SAT_API.Application.DTOs.Ejecuciones;

public class ActualizarEstatusEjecucionRequestDto
{
    public EstatusEjecucion EstatusEjecucion { get; set; }
    public int EjecucionId { get; set; }
    public string Comentario { get; set; } = string.Empty;
}
