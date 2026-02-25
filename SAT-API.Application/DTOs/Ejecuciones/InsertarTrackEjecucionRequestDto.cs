using SAT_API.Domain.Enums;

namespace SAT_API.Application.DTOs.Ejecuciones;

public class InsertarTrackEjecucionRequestDto
{
    public int EjecucionId { get; set; }
    public EstatusEjecucion EstatusEjecucion { get; set; }
}
