namespace SAT_API.Application.DTOs.Ejecuciones;

public class CancelarEjecucionRequestDto
{
    public int EjecucionId { get; set; }
    public string Comentario { get; set; } = string.Empty;
}
