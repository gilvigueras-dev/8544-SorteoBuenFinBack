using SAT_API.Domain.Entities.Databricks;

namespace SAT_API.Domain.Interfaces.Databricks
{
    public interface IDatabricksRepository
    {
        Task<List<DatabricksCluster>> GetClustersAsync();
        Task<DatabricksCluster?> GetClusterAsync(string clusterId);
        Task<JobResponse> GetJobsAsync(long? jobId = null, int limit = 25);
        Task<JobRun> GetJobRunAsync(long runId);
        Task<bool> StartClusterAsync(string clusterId);
        Task<bool> UploadFileToDbfsAsync(string filePath, byte[] fileContent);
        Task<JobExecuteRunResponse> RunJobAsync(long jobId, Dictionary<string, object>? parameters = null);
        Task<JobRun> WaitForJobCompletionAsync(long runId, int delaySeconds = 30, int maxAttempts = 120);
        Task<byte[]> DownloadFileFromDbfsAsync(string dbfsPath);
        Task<string> GetDbfsFilePathAsync(int executionId, int stageId);
        Task StreamFileFromDbfsAsync(string dbfsPath, Stream outputStream);
        Task<StatusFileDownloadedResponse?> GetFileDownloadStatusAsync(StatusFileDownloadedRequest request);
    }
}