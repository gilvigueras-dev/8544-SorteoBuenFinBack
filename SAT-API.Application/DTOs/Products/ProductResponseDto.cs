namespace SAT_API.Application.DTOs.Products;

public class ProductResponseDto
{
    public int IdProducto { get; set; }
    public int IdArchivo { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public bool Estatus { get; set; }
    public bool EstatusRepositorio { get; set; }
     public bool ConsultaCC { get; set; }
    public string? UrlCC { get; set; } // Assuming this is the path to
}
