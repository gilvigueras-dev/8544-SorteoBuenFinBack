namespace SAT_API.Domain.Entities.Products;

public class ProductStatusRequest
{
    [ColumnMap("p_id_ejecucion")]
    public int IdEjecucion { get; set; }

    [ColumnMap("p_id_archivo")]
    public int IdArchivo { get; set; }
}
