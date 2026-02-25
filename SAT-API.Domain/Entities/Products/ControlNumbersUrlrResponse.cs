namespace SAT_API.Domain.Entities.Products;

public class ControlNumbersUrlResponse
{
    [ColumnMap("url")]
    public string Url { get; set; } = string.Empty;
}
