namespace SAT_API.Application.DTOs.Parameters;

public class ParameterInsertRequestDto
{
    public int ExecutionId { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}
