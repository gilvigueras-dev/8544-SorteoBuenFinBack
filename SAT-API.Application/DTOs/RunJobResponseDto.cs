namespace SAT_API.Application.DTOs;

public class RunJobResponseDto
{
    public decimal RunId { get; set; }
    public long NumberInJob { get; set; }
}

public class RunStatusJobResponseDto
{
    public long RunId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RunState { get; set; } = string.Empty;
}