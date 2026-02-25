namespace SAT_API.Application.DTOs.Products
{
    public record MoveFileResponseDto
    {
        public decimal RunId { get; set; }
        public long NumberInJob { get; set; }
    }
}
