using AutoMapper;
using SAT_API.Application.Common;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Ejecuciones;
using SAT_API.Application.DTOs.Track;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Middlewares.Validators.Interfaces;
using SAT_API.Domain.Enums;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services;

public class TrackEjecucionServices : ITrackEjecucionServices
{
    private readonly ITrackEjecucionRepository _trackEjecucionRepository;
    private readonly System.Security.Claims.ClaimsPrincipal _currentUser;
    private readonly ITrackEjecucionInfrastructure _infrastructure;
    private const string ApplicationPermissionDenied = "ApplicationPermissionDenied";
    private const string ErrorMessage = "Error interno del servidor";
    private const string InternalError = "INTERNAL_ERROR";

    public TrackEjecucionServices(
        ITrackEjecucionRepository trackEjecucionRepository,
        IClaimService claimService,
        ITrackEjecucionInfrastructure infrastructure)
    {
        _trackEjecucionRepository = trackEjecucionRepository ?? throw new ArgumentNullException(nameof(trackEjecucionRepository));
        _currentUser = claimService.GetCurrentUser() ?? throw new ArgumentNullException(nameof(claimService));
        _infrastructure = infrastructure ?? throw new ArgumentNullException(nameof(infrastructure));
    }

    public async Task<Result<List<InsertarTrackEjecucionDto>>> InsertarTrackEjecucionAsync(InsertarTrackEjecucionRequestDto request)
    {
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
                   return Result<List<InsertarTrackEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionDenied]);
                }
            }
            else
            {
              return Result<List<InsertarTrackEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionDenied]);
            }
        }
        else
        {
           return Result<List<InsertarTrackEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionDenied]);
        }
        
        try
        {
            var estatusList = await _trackEjecucionRepository.InsertarTrackEjecucionAsync(request.EjecucionId, (int)request.EstatusEjecucion);
            var result = _infrastructure.Mapper.Map<List<InsertarTrackEjecucionDto>>(estatusList);
            return Result<List<InsertarTrackEjecucionDto>>.Success(result, "Se ha actualizo el estatus de la ejecución.");
        }
        catch (Exception ex)
        {
            return Result<List<InsertarTrackEjecucionDto>>.Failure(
                ErrorMessage,
                InternalError,
                ex.Message
            );
        }
    }

    public async Task<Result<List<TrackEjecucionDto>>> ObtenerTrackEjecucionAsync(int ejecucionId)
    {   
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
                    return Result<List<TrackEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionDenied]);
                }
            }
            else
            {
               return Result<List<TrackEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionDenied]);
            }
        }
        else
        {
           return Result<List<TrackEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionDenied]);
        }

        try
        {
            var lista = await _trackEjecucionRepository.ObtenerTrackEjecucionAsync(ejecucionId);
            var result = _infrastructure.Mapper.Map<List<TrackEjecucionDto>>(lista);
            return Result<List<TrackEjecucionDto>>.Success(result, "Correcto, lista de track ejecución");
        }
        catch (Exception ex)
        {
            return Result<List<TrackEjecucionDto>>.Failure(
                ErrorMessage,
                InternalError,
                ex.Message
            );
        }
    }

    public async Task<Result<DataPopulationValidationResponseDto?>> DataPopulationValidation(DataPopulationValidationRequestDto request)
    {
        // Validate the request using the validation service
        await _infrastructure.ValidationService.ValidateAndThrowAsync(request);
        try
        {
            var response = await _trackEjecucionRepository.DataPopulationValidation(_infrastructure.Mapper.Map<Domain.Entities.Track.DataPopulationValidationRequest>(request));
            var result = _infrastructure.Mapper.Map<DataPopulationValidationResponseDto?>(response);
            if (result == null)
            {
                return Result<DataPopulationValidationResponseDto?>.Failure(
                    "No se encontró información de validación de población de datos.",
                    "NOT_FOUND",
                    "No se encontraron resultados para la validación solicitada."
                );
            }
            return Result<DataPopulationValidationResponseDto?>.Success(result, "Validación de población de datos exitosa.");
        }
        catch (Exception ex)
        {
            return Result<DataPopulationValidationResponseDto?>.Failure(
                ErrorMessage,
                InternalError,
                ex.Message
            );
        }
    }

    public async Task<Result<RunJobResponseDto>> DataPopulation(DataPopulationRequestDto request)
    {
        // Validate the request using the validation service
        await _infrastructure.ValidationService.ValidateAndThrowAsync(request);
        try
        {
            var jobId = await _infrastructure.JobConfigRepository.GetByNombreAsync("PoblarTablaEtapa");
            if (jobId == null)
            {
                return Result<RunJobResponseDto>.Failure(
                    "No se encontró la configuración del trabajo para poblar la tabla de etapa.",
                    "NOT_FOUND",
                    "No se encontró la configuración del trabajo para poblar la tabla de etapa."
                );
            }

            var result = await _infrastructure.DatabricksRepository.RunJobAsync(
                long.Parse(jobId.JobId),
                new Dictionary<string, object>
                {
                    { "etapa", request.StageId },
                    { "id_ejecucion", request.ExecutionId },
                    { "rfc", _currentUser.GetRFC() },
                    { "ip", _infrastructure.AddressClientService.ObtenerIPCliente() }
                }
            );

            if (result != null)
            {
                return Result<RunJobResponseDto>.Success(new RunJobResponseDto { NumberInJob = result.NumberInJob, RunId = result.RunId }, "Población de datos iniciada correctamente.");
            }
            else
            {
                return Result<RunJobResponseDto>.Failure(
                    "Error al iniciar la población de datos.",
                    "JOB_RUN_ERROR",
                   $"No se pudo iniciar el job {jobId} de población de datos."
                );
            }
        }
        catch (Exception ex)
        {
            return Result<RunJobResponseDto>.Failure(
                ErrorMessage,
                InternalError,
                ex.Message
            );
        }
    }
}
