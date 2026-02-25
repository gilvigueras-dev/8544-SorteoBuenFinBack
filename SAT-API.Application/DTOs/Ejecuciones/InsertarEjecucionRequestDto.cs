namespace SAT_API.Application.DTOs.Ejecuciones;

public class InsertarEjecucionRequestDto
{
    
    public string Nombre { get; set; } = string.Empty;

       public DateTime Fecha { get; set; }

   
    public int IdEstatusEjecucion { get; set; }
}
