using System.ComponentModel.DataAnnotations;

namespace SAT_API.Application.DTOs;

public class RunJobRequestDto
{
    [MaxLength(100)]
    public string JobName { get; set; } = string.Empty;
    public Dictionary<string, object>? Parameters { get; set; } = new Dictionary<string, object>();
}
