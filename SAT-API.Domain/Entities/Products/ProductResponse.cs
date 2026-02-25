namespace SAT_API.Domain.Entities.Products;

public class ProductResponse
{
    [ColumnMap("id_producto")]
    public int IdProducto { get; set; }
    [ColumnMap("id_archivo")]
    public int IdArchivo { get; set; }
    [ColumnMap("nombre_archivo")]
    public string NombreArchivo { get; set; } = string.Empty;
    [ColumnMap("ruta")]
    public string RutaArchivo { get; set; } = string.Empty;
    [ColumnMap("estatus")]
    public bool Estatus { get; set; }
    [ColumnMap("status_repositorio")]
    public bool EstatusRepositorio { get; set; }
    [ColumnMap("cc_generadas")]
    public bool CCGenerated { get; set; }
    [ColumnMap("ruta_cc")]
    public string? CCPath { get; set; }
}
