namespace SAT_API.Application.DTOs.Products;

public class ControlNumbersResponseDto
{
     public DateTime Date { get; set; }
    public string Product { get; set; } = string.Empty;
    public string ControlNumberName { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
