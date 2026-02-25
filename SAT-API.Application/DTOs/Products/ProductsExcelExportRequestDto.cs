namespace SAT_API.Application.DTOs.Products
{
    public class ProductsExcelExportRequestDto
    {
        public string? Stage { get; set; }
        public string? Action { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? RFC { get; set; }
    }
}