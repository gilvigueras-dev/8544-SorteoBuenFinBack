using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SAT_API.Domain.Entities.Databricks;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Infrastructure.Data;
using System.Text;
using System.Text.Json;
using System.Threading;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SAT_API.Infrastructure.Repositories.Databricks
{
    public class DatabricksRepository : IDatabricksRepository
    {
        private readonly IDatabricksClient _databricksClient;
        private readonly ILogger<DatabricksRepository> _logger;
        private readonly IDbContext _dbContext;
        private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public DatabricksRepository(IDatabricksClient client, ILogger<DatabricksRepository> logger, IDbContext dbContext)
        {
            _databricksClient = client;
            _logger = logger;
            _dbContext = dbContext;
        }

        public Task<DatabricksCluster?> GetClusterAsync(string clusterId)
        {
            throw new NotImplementedException();
        }

        public Task<List<DatabricksCluster>> GetClustersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<JobResponse> GetJobsAsync(long? jobId = null, int limit = 25)
        {
            var queryParams = new List<string> { $"limit={limit}" };
            if (jobId.HasValue)
                queryParams.Add($"job_id={jobId}");

            var query = string.Join("&", queryParams);
            var httpClient = _databricksClient.GetClient();
            var response = await httpClient.GetAsync($"/api/2.2/jobs/get?{query}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var jobResponse = JsonSerializer.Deserialize<JobResponse>(json, s_jsonOptions);

            return jobResponse ?? throw new InvalidOperationException("No se pudo deserializar la respuesta");
        }

        public async Task<JobRun> GetJobRunAsync(long runId)
        {
            var queryParams = new List<string> { $"run_id={runId}" };
            var query = string.Join("&", queryParams);
            var httpClient = _databricksClient.GetClient();
            var response = await httpClient.GetAsync($"/api/2.2/jobs/runs/get?{query}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var jobResponse = JsonSerializer.Deserialize<JobRun>(json, s_jsonOptions);

            return jobResponse ?? throw new InvalidOperationException("No se pudo deserializar la respuesta");
        }

        public async Task<JobExecuteRunResponse> RunJobAsync(long jobId, Dictionary<string, object>? parameters = null)
        {
            var requestBody = new
            {
                job_id = jobId,
                job_parameters = parameters ?? new Dictionary<string, object>()
            };

            var json = JsonSerializer.Serialize(requestBody, s_jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = _databricksClient.GetClient();
            var response = await httpClient.PostAsync($"/api/2.1/jobs/run-now", content);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _logger.LogError("Job with ID {JobId} not found.", jobId);
                throw new KeyNotFoundException($"Job with ID {jobId} not found.");
            }

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JobExecuteRunResponse>(responseContent, s_jsonOptions);

            return result ?? throw new InvalidOperationException("No se pudo deserializar la respuesta");
        }

        public Task<bool> StartClusterAsync(string clusterId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadFileToDbfsAsync(string filePath, byte[] fileContent)
        {
            throw new NotImplementedException();
        }
        public async Task<JobRun> WaitForJobCompletionAsync(long runId, int delaySeconds = 30, int maxAttempts = 120)
        {
            var jobRun = await GetJobRunAsync(runId);

            // Si ya está terminado, retornamos el resultado
            if (jobRun.State.LifeCicleState == "TERMINATED" || jobRun.State.ResultState == "SUCCESS")
            {
                return jobRun;
            }

            // Verificar si hemos alcanzado el máximo de intentos
            if (maxAttempts <= 0)
            {
                throw new TimeoutException($"El job {runId} no terminó después del tiempo máximo de espera");
            }

            // Esperar antes de la siguiente verificación
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

            // Llamada recursiva con un intento menos
            return await WaitForJobCompletionAsync(runId, delaySeconds, maxAttempts - 1);
        }
        public async Task StreamFileFromDbfsAsync(string dbfsPath, Stream outputStream)
        {
            var httpClient = _databricksClient.GetClient();
            httpClient.Timeout = TimeSpan.FromMinutes(30);
            dbfsPath = Uri.UnescapeDataString(dbfsPath.Trim());
            string fullPath = dbfsPath.StartsWith('/') ? dbfsPath : $" /{dbfsPath}";
            if (!fullPath.StartsWith("/FileStore/"))
            {
                fullPath = $"/FileStore/shared_uploads{fullPath}";
            }
            _logger.LogInformation("Streaming file using DBFS API: {FilePath}", fullPath);
            long offset = 0;
            int chunkSize = 524288; // This has a limit of 1 MB, and a default value of 0.5 MB.
            bool hasMore = true;
            while (hasMore)
            {
                var queryParams = new List<string>
                {
                    $"path={Uri.EscapeDataString(fullPath)}",
                    $"offset={offset}",
                    $"length={chunkSize}"
                };
                var queryString = string.Join("&", queryParams);
                var requestUrl = $"api/2.0/dbfs/read?{queryString}";
                _logger.LogDebug("DBFS API request: offset {SetOff}", offset);
                using (var response = await httpClient.GetAsync(requestUrl))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("DBFS API error. StatusCode: {StatusCode}, Error: {ErrorContent}", (int)response.StatusCode,errorContent);
                        throw new HttpRequestException("DBFS API error: {errorContent}");
                    }
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<DbfsReadResponse>(responseContent);
                    if (result != null && !string.IsNullOrEmpty(result.data))
                    {
                        var chunkBytes = Convert.FromBase64String(result.data);
                        await outputStream.WriteAsync(chunkBytes.AsMemory());
                        await outputStream.FlushAsync();
                        offset += result.bytes_read;
                        hasMore = result.bytes_read == chunkSize;
                        _logger.LogDebug("Streamed chunk:{Result} bytes, total:{Set}", result.bytes_read, offset);
                        if (result.bytes_read == 0)
                        {
                            hasMore = false;
                        }
                    }
                    else
                    {
                        _logger.LogError("DBFS API response deserialization failed");
                        throw new InvalidOperationException("DBFS API response failed");
                    }
                }
            }
            _logger.LogInformation("File streamed successfully: {OffSet} bytes total", offset);
        }
        public async Task<byte[]> DownloadFileFromDbfsAsync(string dbfsPath)
        {
            var httpClient = _databricksClient.GetClient();

            dbfsPath = Uri.UnescapeDataString(dbfsPath.Trim());

            // Asegurar que el path tenga el formato correcto para DBFS API
            string fullPath = dbfsPath.StartsWith('/') ? dbfsPath : $" /{dbfsPath}";
            if (!fullPath.StartsWith("/FileStore/"))
            {
                fullPath = $"/FileStore/shared_uploads{fullPath}";
            }

            _logger.LogInformation("Downloading file using DBFS API: {FilePath}", fullPath);

            var chunks = new List<byte>();
            long offset = 0;
            int chunkSize = 1048576; // 1MB
            bool hasMore = true;

            while (hasMore)
            {
                // Construir la URL con query parameters
                var queryParams = new List<string>
                {
                    $"path={Uri.EscapeDataString(fullPath)}",
                    $"offset={offset}",
                    $"length={chunkSize}"
                };

                var queryString = string.Join("&", queryParams);
                var requestUrl = $"api/2.0/dbfs/read?{queryString}";

                _logger.LogInformation("DBFS API GET request to: {UrlRequest}", requestUrl);

                var response = await httpClient.GetAsync(requestUrl);

                _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DBFS API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"DBFS API error: {errorContent}");
                }

                // CAMBIO PRINCIPAL: Leer como bytes directamente
                var responseBytes = await response.Content.ReadAsByteArrayAsync();

                // Si la respuesta es JSON (como debería ser con DBFS), deserializar
                var responseContent = Encoding.UTF8.GetString(responseBytes);
                var result = JsonConvert.DeserializeObject<DbfsReadResponse>(responseContent);

                if (result != null && !string.IsNullOrEmpty(result.data))
                {
                    var chunkBytes = Convert.FromBase64String(result.data);
                    chunks.AddRange(chunkBytes);

                    offset += result.bytes_read;
                    hasMore = result.bytes_read == chunkSize;

                    _logger.LogInformation("Chunk downloaded: {ResultByte} bytes, offset now: {OffSet}", result.bytes_read, offset);

                    // Si no se leyeron bytes, salir del loop para evitar bucle infinito
                    if (result.bytes_read == 0)
                    {
                        hasMore = false;
                    }
                }
                else
                {
                    _logger.LogError("DBFS API response deserialization failed or data is missing.");
                    throw new InvalidOperationException("DBFS API response deserialization failed or data is missing.");
                }
            }

            _logger.LogInformation("File downloaded successfully: {Count} bytes total", chunks.Count);
            return chunks.ToArray();
        }
        public async Task<string> GetDbfsFilePathAsync(int executionId, int stageId)
        {
            var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.fn_obtener_informe_validacion_por_etapa(@p_id_ejecucion,@p_etapa)";
            var parameters = new
            {
                p_id_ejecucion = executionId,
                p_etapa = stageId
            };
            var connection = _dbContext.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);
            return result ?? string.Empty;
        }
        public async Task<StatusFileDownloadedResponse?> GetFileDownloadStatusAsync(StatusFileDownloadedRequest request)
        {
            var query = $"SELECT * FROM {ConstantsSchemas.dec08544_dd_sac_svsbf}.ffn_obtener_estado_descarga(@p_id_ejecucion, @p_id_etapa)";
            var connection = _dbContext.CreateConnection();
            var result = await connection.GetFirstOrDefaultAsync<StatusFileDownloadedResponse, StatusFileDownloadedRequest>(query, request);
            return result;
        }
    }
}