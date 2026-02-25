using AutoMapper;
using SAT_API.Application.Common;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Insumos;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Enums;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services;

public class InsumoService : IInsumoService
{
    #region Constructor
    private readonly IInsumoRepository _insumoRepository;
    private readonly IPistaAuditoriaServices _pistaAuditoriaServices;
    private readonly System.Security.Claims.ClaimsPrincipal _currentUser;
    private readonly IInsumoInfrastructure _infrastructure;
    private const string ApplicationPermissionError = "ApplicationInsumoPermissionDenied";
    private const string ErrorInternal = "Error interno del servidor";
    private const string ErrorInternalLog = "INTERNAL_ERROR";

    public InsumoService(
        IInsumoRepository insumoRepository,
        IPistaAuditoriaServices pistaAuditoriaServices,
        IClaimService claimService,
        IInsumoInfrastructure infrastructure)
    {
        _insumoRepository = insumoRepository ?? throw new ArgumentNullException(nameof(insumoRepository));
        _pistaAuditoriaServices = pistaAuditoriaServices ?? throw new ArgumentNullException(nameof(pistaAuditoriaServices));
        _currentUser = claimService.GetCurrentUser() ?? throw new ArgumentNullException(nameof(claimService));
        _infrastructure = infrastructure ?? throw new ArgumentNullException(nameof(infrastructure));
    }
    #endregion

    public async Task<Result<RunJobResponseDto>> GenerateStageResources(int ejecucionId, int etapa)
    {
        // Check if the user has the required permissions
        var roles = await _infrastructure.RoleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario }.Contains(role.RoleName))
                {
                    return Result<RunJobResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<RunJobResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<RunJobResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        try
        {
            var jobName = "GetInsumosEtapa1";
            var jobConfig = await _infrastructure.JobConfigService.GetByNombreAsync(jobName);

            if (jobConfig == null)
            {
                return Result<RunJobResponseDto>.Failure(
                    "JobId not found on database",
                    "CONFIG_INSUMO__NOT_FOUND",
                    $"No se encontró JobId: {jobName}"
                );
            }

            long jobId = (long)Convert.ToDecimal(jobConfig.JobId);
            var jobParametros = new Dictionary<string, object>()
            {
                { "id_ejecucion", ejecucionId },
                { "etapa", etapa },
                { "rfc", _currentUser.GetRFC()},
                { "ip", _infrastructure.AddressClientService.ObtenerIPCliente() }
            };

            var resultado = await _infrastructure.DatabricksRepository.RunJobAsync(jobId, jobParametros);
            if (resultado == null)
            {
                return Result<RunJobResponseDto>.Failure(
                    "Error al ejecutar el job en Databricks",
                    "JOB_EXECUTION_ERROR",
                    $"El job {jobName} no se completó exitosamente"
                );
            }
            await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = ejecucionId, Actividad = $"GenerarRecursos - {etapa}", Descripcion = $"Se generan los recursos de la etapa: {etapa}, ejecución: {ejecucionId}" });

            return Result<RunJobResponseDto>.Success(new RunJobResponseDto { RunId = resultado.RunId, NumberInJob = resultado.NumberInJob }, "Recursos generados exitosamente");
        }
        catch (Exception ex)
        {
            return Result<RunJobResponseDto>.Failure(
                ErrorInternal,
                ErrorInternalLog,
                ex.Message
            );
        }
    }

    public async Task<Result<List<InsumoDto>>> GetOutputResourcesAsync(int ejecucionId, int etapa)
    {
        // Check if the user has the required permissions
        var roles = await _infrastructure.RoleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario, RolesUsuario.Invitado }.Contains(role.RoleName))
                {
                    return Result<List<InsumoDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<List<InsumoDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<InsumoDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        var listaInsumos = await _insumoRepository.GetInsumosAsync(ejecucionId, etapa);
        if (listaInsumos == null || !listaInsumos.Any())
        {
            return Result<List<InsumoDto>>.Failure(
                "No se encontraron insumos para la ejecución y etapa especificadas",
                "NO_INSUMOS_FOUND",
                $"No se encontraron insumos para la ejecución: {ejecucionId}, etapa: {etapa}"
            );
        }

        List<InsumoDto> listaInsumosDto = _infrastructure.Mapper.Map<List<InsumoDto>>(listaInsumos);
        await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = ejecucionId, Actividad = $"InsumosEtapa - {etapa}", Descripcion = $"Se obtienen los insumos de la etapa: {etapa}, ejecución: {ejecucionId}" });
        return Result<List<InsumoDto>>.Success(listaInsumosDto, "Insumos obtenidos exitosamente");
    }

    public async Task<Result<List<InsumoDto>>> GetInsumosEntradaAsync(int ejecucionId, int etapa)
    {
        try
        {
            var jobName = "GetInsumosEtapa1";

            var jobConfig = await _infrastructure.JobConfigService.GetByNombreAsync(jobName);

            if (jobConfig == null)
            {
                return Result<List<InsumoDto>>.Failure(
                    "JobId not found on database",
                    "CONFIG_INSUMO__NOT_FOUND",
                    $"No se encontró JobId: {jobName}"
                );
            }

            long jobId = (long)Convert.ToDecimal(jobConfig.JobId);

            var jobParametros = new Dictionary<string, object>()
            {
                { "id_ejecucion", ejecucionId },
                { "etapa", etapa },
                { "rfc", _currentUser.GetRFC()},
                { "ip", _infrastructure.AddressClientService.ObtenerIPCliente() }
            };

            var resultado = await _infrastructure.DatabricksRepository.RunJobAsync(jobId, jobParametros);
            var completedJob = await _infrastructure.DatabricksRepository.WaitForJobCompletionAsync(long.Parse(resultado.RunId.ToString()));
            if (completedJob.State.ResultState == "SUCCESS")
            {
                var listaInsumos = await _insumoRepository.GetInsumosAsync(ejecucionId, etapa);

                List<InsumoDto> listaInsumosDto = _infrastructure.Mapper.Map<List<InsumoDto>>(listaInsumos);
                await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = ejecucionId, Actividad = $"InsumosEtapa - {etapa}", Descripcion = $"Se obtienen los insumos de la etapa: {etapa}, ejecución: {ejecucionId}" });
                return Result<List<InsumoDto>>.Success(listaInsumosDto, "Insumos obtenidos exitosamente");
            }
            return Result<List<InsumoDto>>.Failure(
                "Error al ejecutar el job en Databricks",
                "JOB_EXECUTION_ERROR",
                $"El job {jobName} no se completó exitosamente"
            );
        }
        catch (Exception ex)
        {
            return Result<List<InsumoDto>>.Failure(
                ErrorInternal,
                ErrorInternalLog,
                ex.Message
            );
        }
    }

    public async Task<Result<ProcesoValidacionDto>> GetValidarInsumos(int ejecucionId, int etapa)
    {
        // Check if the user has the required permissions
        var roles = await _infrastructure.RoleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario }.Contains(role.RoleName))
                {
                    return Result<ProcesoValidacionDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<ProcesoValidacionDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<ProcesoValidacionDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        try
        {
            var jobName = "ValidarLayout";

            if (string.IsNullOrEmpty(jobName))
            {
                return Result<ProcesoValidacionDto>.Failure(
                    $"{jobName} not found on app config",
                    "CONFIG_INSUMO_NOT_FOUND",
                    $"{jobName} not found on app config"
                );
            }

            var jobConfig = await _infrastructure.JobConfigService.GetByNombreAsync(jobName);

            if (jobConfig == null)
            {
                return Result<ProcesoValidacionDto>.Failure(
                    "JobId not found on database",
                    "CONFIG_INSUMO__NOT_FOUND",
                    $"No se encontró JobId: {jobName}"
                );
            }

            long jobId = (long)Convert.ToDecimal(jobConfig.JobId);
            var jobParametros = new Dictionary<string, object>
            {
                { "id_ejecucion", ejecucionId },
                { "etapa", etapa },
                { "rfc", _currentUser.GetRFC()},
                { "ip", _infrastructure.AddressClientService.ObtenerIPCliente() }
            };

            var resultado = await _infrastructure.DatabricksRepository.RunJobAsync(jobId, jobParametros);

            var trackEjecucionEtapa = etapa switch
            {
                1 => EstatusEjecucion.Etapa1_ValidacionInsumos,
                2 => EstatusEjecucion.Etapa2_ValidacionInsumos,
                3 => EstatusEjecucion.Etapa3_ValidacionInsumos,
                4 => EstatusEjecucion.Etapa4_ValidacionInsumos,
                _ => throw new ArgumentException("The stage etapa is not valid")
            };

            await _infrastructure.TrackEjecucionRepository.InsertarTrackEjecucionAsync(ejecucionId, (int)trackEjecucionEtapa);
            await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = ejecucionId, Actividad = $"ValidarInsumos - {etapa}", Descripcion = $"Se obtienen los insumos de la etapa: {etapa}, ejecución: {ejecucionId}" });
            return Result<ProcesoValidacionDto>.Success(new ProcesoValidacionDto { NumberInJob = resultado.NumberInJob, RunId = resultado.RunId }, "Proceso ejecutado exitosamente");
        }
        catch (Exception ex)
        {
            return Result<ProcesoValidacionDto>.Failure(
                ErrorInternal,
                ErrorInternalLog,
                ex.Message
            );
        }
    }

    public async Task<Result<List<EstatusValidacionDto>>> GetEstatusValidacionAsync(int ejecucionId, int etapa)
    {
        try
        {
            var estatusList = await _insumoRepository.GetEstatusValidacionAsync(ejecucionId, etapa);
            var listaValidacion = _infrastructure.Mapper.Map<List<EstatusValidacionDto>>(estatusList);
            await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = ejecucionId, Actividad = $"EstatusValidacion - {etapa}", Descripcion = $"Se obtienen el estatus de la validación de la etapa: {etapa}, ejecución: {ejecucionId}" });
            return Result<List<EstatusValidacionDto>>.Success(listaValidacion, "Estatus obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            return Result<List<EstatusValidacionDto>>.Failure(
                ErrorInternal,
                ErrorInternalLog,
                ex.Message
            );
        }
    }

    public async Task<Result<RunStatusJobResponseDto>> StatusJobResourceAsync(long runId)
    {
        // Check if the user has the required permissions
        var roles = await _infrastructure.RoleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario }.Contains(role.RoleName))
                {
                    return Result<RunStatusJobResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<RunStatusJobResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<RunStatusJobResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        var jobStatus = await _infrastructure.DatabricksRepository.GetJobRunAsync(runId);
        if (jobStatus == null)
        {
            return Result<RunStatusJobResponseDto>.Failure(
                "Job not found",
                "JOB_NOT_FOUND",
                $"No se encontró el job con RunId: {runId}"
            );
        }

        var jobRun = _infrastructure.Mapper.Map<RunStatusJobResponseDto>(jobStatus);
        return Result<RunStatusJobResponseDto>.Success(jobRun, "Job status retrieved successfully"
        );
    }
}