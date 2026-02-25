namespace SAT_API.Application.DTOs.Insumos;

public record EstatusValidacionDto
{
    public string Archivo { get; set; } = string.Empty;
    public bool Validado { get; set; }
    public bool Existe { get; set; }
    public int ArchivoId { get; set; }
    public bool Warning { get; set; }
    public bool ConsultaCC { get; set; }
    public string? UrlCC { get; set; }
    public bool Descargado { get; set; }
}
