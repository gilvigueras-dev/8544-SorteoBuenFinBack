namespace SAT_API.Application.DTOs;

public class FileStreamResponseDto
{
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long? FileSize { get; set; } 
}
