namespace SAT_API.Domain.Entities.Products;

public class ControlNumbersFileResponse
{
    [ColumnMap("fecha")]
    public DateTime Date { get; set; }
    [ColumnMap("producto")]
    public string ProductName { get; set; } = string.Empty;
    [ColumnMap("nombre_cifra")]
    public string ControlNumber { get; set; } = string.Empty;
    [ColumnMap("valor")]
    public decimal Value { get; set; }
}
