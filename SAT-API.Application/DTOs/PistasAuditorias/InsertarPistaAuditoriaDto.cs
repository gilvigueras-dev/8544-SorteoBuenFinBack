namespace SAT_API.Application.DTOs.PistasAuditorias;

public class InsertarPistaAuditoriaDto
{
    public int EjecucionId { get; set; }
    public string Actividad { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
