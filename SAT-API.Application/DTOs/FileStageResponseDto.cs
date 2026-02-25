namespace SAT_API.Application.DTOs;

public class FileStageResponseDto
{
    public byte[]? FileContent { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream"; // Default content type
}
