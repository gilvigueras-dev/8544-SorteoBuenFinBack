namespace SAT_API.Application.DTOs.Ejecuciones;

public class ActualizarEstatusEjecucionIdDto
{
    public int EjecucionId { get; set; }
    public int EstatusEjecucionId { get; set; }
    public string? Comentario { get; set; }
}
