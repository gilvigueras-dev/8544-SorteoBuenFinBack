namespace SAT_API.Domain.Entities.Products
{
    public class ProductsExcelExportRequest
    {
        [ColumnMap("p_etapa")]
        public int? Stage { get; set; }

        [ColumnMap("p_accion")]
        public string? Action { get; set; }

        [ColumnMap("p_fecha_inicio")]
        public DateTime? StartDate { get; set; }

        [ColumnMap("p_fecha_final")]
        public DateTime? EndDate { get; set; }

        [ColumnMap("p_rfc")]
        public string? RFC { get; set; }
    }
}