namespace SAT_API.Application.DTOs.Parameters;

public class ParameterGenericResponseDto
{
    public int ParameterId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
