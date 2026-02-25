namespace SAT_API.Application.DTOs.Products;

public class ControlNumbersFileResponseDto
{
    public DateTime Date { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ControlNumber { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
