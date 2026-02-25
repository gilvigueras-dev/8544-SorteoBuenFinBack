namespace SAT_API.Application.DTOs.Track
{
    public record DataPopulationResponseDto
    {
        public decimal RunId { get; set; }
        public long NumberInJob { get; set; }
    }
}
