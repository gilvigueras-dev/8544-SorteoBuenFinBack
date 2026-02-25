using AutoMapper;
using SAT_API.Application.Common;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Types;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Interfaces.Databricks;
using SAT_API.Domain.Entities.Databricks;
using SAT_API.Domain.Enums;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces.UserOperations;
using System.Reflection.Metadata;

namespace SAT_API.Application.Services.Databricks;

public class DatabricksService : IDatabricksService
{
    private readonly IDatabricksRepository _databricksRepository;
    private readonly IJobConfigRepository _jobConfigRepository;
    private readonly ITranslator _translator;
    private readonly System.Security.Claims.ClaimsPrincipal _currentUser;
    private readonly IAddressClientService _addressClientService;
    private readonly IRoleManagementRepository _roleManagementRepository;
    private readonly IMapper _mapper;
    private const string PermissionDeniedError = "ApplicationProductsPermissionDenied";

    public DatabricksService(IDatabricksRepository databricksRepository, IJobConfigRepository jobConfigRepository, ITranslator translator, 
        IClaimService claimService, IAddressClientService addressClientService, IRoleManagementRepository roleManagementRepository, IMapper mapper)
    {
        _databricksRepository = databricksRepository;
        _jobConfigRepository = jobConfigRepository;
        _translator = translator;
        _mapper = mapper;
        _addressClientService = addressClientService;
        _currentUser = claimService.GetCurrentUser();
        _roleManagementRepository = roleManagementRepository;
    }

    public async Task<Result<FileStageResponseDto>> DownloadFileFromDbfsAsync(string dbfsPath)
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario, RolesUsuario.Invitado }.Contains(role.RoleName))
                {
                    return Result<FileStageResponseDto>.Failure(_translator[PermissionDeniedError]);
                }
            }
            else
            {
                return Result<FileStageResponseDto>.Failure(_translator[PermissionDeniedError]);
            }
        }
        else
        {
            return Result<FileStageResponseDto>.Failure(_translator[PermissionDeniedError]);
        }

        if (string.IsNullOrEmpty(dbfsPath))
        {
            return Result<FileStageResponseDto>.Failure(_translator["ApplicationDatabricksDownloadFileFromDbfsAsyncDbfsPathNull"]);
        }

        var fileContent = await _databricksRepository.DownloadFileFromDbfsAsync(dbfsPath);
        if (fileContent == null)
        {
            return Result<FileStageResponseDto>.Failure(_translator["ApplicationDatabricksDownloadFileFromDbfsAsyncFileContentNull"]);
        }

        // Extraer nombre del archivo desde la ruta si no se proporciona
        var finalFileName = Path.GetFileName(Uri.UnescapeDataString(dbfsPath.Trim())) ?? "downloaded_file";
        // Determinar content type basado en la extensión
        var contentType = ContentTypes.GetContentType(finalFileName);
        return Result<FileStageResponseDto>.Success(new FileStageResponseDto
        {
            FileContent = fileContent,
            FileName = $"{finalFileName}",
            ContentType = contentType
        });
    }

    public async Task StreamFileFromDbfsAsync(string dbfsPath, Stream outputStream)
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario, RolesUsuario.Invitado }.Contains(role.RoleName))
                {
                    throw new UnauthorizedAccessException(_translator[PermissionDeniedError]);
                }
            }
            else
            {
                throw new UnauthorizedAccessException(_translator[PermissionDeniedError]);
            }
        }
        else
        {
            throw new UnauthorizedAccessException(_translator[PermissionDeniedError]);
        }

        if (string.IsNullOrEmpty(dbfsPath))
        {
            throw new ArgumentException(_translator["ApplicationDatabricksDownloadFileFromDbfsAsyncDbfsPathNull"]);
        }

        await _databricksRepository.StreamFileFromDbfsAsync(dbfsPath, outputStream);
    }

    public async Task<Result<string>> GetDbfsFilePathByStageAsync(DownloadFileStageRequestDto request)
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario, RolesUsuario.Invitado }.Contains(role.RoleName))
                {
                    return Result<string>.Failure(_translator[PermissionDeniedError]);
                }
            }
            else
            {
                return Result<string>.Failure(_translator[PermissionDeniedError]);
            }
        }
        else
        {
            return Result<string>.Failure(_translator[PermissionDeniedError]);
        }

        var dbfsPath = await _databricksRepository.GetDbfsFilePathAsync(request.ExecutionId, request.StageId);
        if (string.IsNullOrEmpty(dbfsPath))
        {
            return Result<string>.Failure(_translator["ApplicationDatabricksGetDbfsFilePathAsyncFailure"]);
        }
        return Result<string>.Success(dbfsPath);
    }

    public async Task<Result<RunJobResponseDto>> RunJobAsync(RunJobRequestDto request)
    {
        if (string.IsNullOrEmpty(request.JobName))
        {
            return Result<RunJobResponseDto>.Failure(_translator["ApplicationDatabricksRunJobAsyncJobIdNull"]);
        }

        var jobInfo = await _jobConfigRepository.GetByNombreAsync(request.JobName);
        if (jobInfo == null)
        {
            return Result<RunJobResponseDto>.Failure(_translator["ApplicationDatabricksRunJobAsyncJobNotFound"]);
        }

        if (request.Parameters == null)
        {
            request.Parameters = new Dictionary<string, object>();
        }

        request.Parameters.Add("rfc", _currentUser?.GetRFC() ?? string.Empty);
        request.Parameters.Add("ip", _addressClientService.ObtenerIPCliente());

        var jobResultExecution = await _databricksRepository.RunJobAsync(long.Parse(jobInfo.JobId), request.Parameters);
        if (jobResultExecution == null)
        {
            return Result<RunJobResponseDto>.Failure(_translator["ApplicationDatabricksRunJobAsyncExecutionFailed"]);
        }

        return Result<RunJobResponseDto>.Success(new RunJobResponseDto
        {
            RunId = jobResultExecution.RunId,
            NumberInJob = jobResultExecution.NumberInJob
        });
    }
    public async Task<Result<FileStatusDownloadedResponseDto>> GetFileDownloadStatusAsync(FileStatusDownloadedRequestDto request)
    {
        var statusRequest = _mapper.Map<StatusFileDownloadedRequest>(request);
        var statusResponse = await _databricksRepository.GetFileDownloadStatusAsync(statusRequest);
        if (statusResponse == null)
        {
            return Result<FileStatusDownloadedResponseDto>.Failure(_translator["ApplicationDatabricksGetFileDownloadStatusAsyncStatusNull"]);
        }

        return Result<FileStatusDownloadedResponseDto>.Success(new FileStatusDownloadedResponseDto
        {
            Status = statusResponse.Status
        });
    }
}