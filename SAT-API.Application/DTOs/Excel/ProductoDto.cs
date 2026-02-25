namespace SAT_API.Application.DTOs.Excel;

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
}

public class ExcelExportRequestDto
{
    public string NombreArchivo { get; set; } = "Reporte";
    public List<string> Columnas { get; set; } = new List<string>();
    public Dictionary<string, object> Filtros { get; set; } = new Dictionary<string, object>();
}
