using SAT_API.Application.Common;
using SAT_API.Application.DTOs;

namespace SAT_API.Application.Interfaces.Databricks;

public interface IDatabricksService
{
    Task<Result<FileStageResponseDto>> DownloadFileFromDbfsAsync(string dbfsPath);
    Task<Result<string>> GetDbfsFilePathByStageAsync(DownloadFileStageRequestDto request);
    Task<Result<RunJobResponseDto>> RunJobAsync(RunJobRequestDto request);
    Task StreamFileFromDbfsAsync(string dbfsPath, Stream outputStream);
    Task<Result<FileStatusDownloadedResponseDto>> GetFileDownloadStatusAsync(FileStatusDownloadedRequestDto request);
}
