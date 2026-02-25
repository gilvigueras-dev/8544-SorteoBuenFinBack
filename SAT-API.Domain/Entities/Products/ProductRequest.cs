namespace SAT_API.Domain.Entities.Products;

public class ProductRequest
{
    [ColumnMap("p_id_ejecucion")]
    public int IdEjecucion { get; set; }
    [ColumnMap("p_id_etapa")]
    public int IdEtapa { get; set; }
}
