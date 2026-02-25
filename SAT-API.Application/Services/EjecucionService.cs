using AutoMapper;
using SAT_API.Application.Common;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Ejecuciones;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.DTOs.Process;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Entities;
using SAT_API.Domain.Entities.Process;
using SAT_API.Domain.Enums;
using SAT_API.Domain.Interfaces;
using SAT_API.Domain.Interfaces.Databricks;
using SAT_API.Domain.Interfaces.UserOperations;

namespace SAT_API.Application.Services;

public class EjecucionService : IEjecucionService
{
    private readonly IEjecucionRepository _ejecucionRepository;
    private readonly IJobConfigRepository _jobConfigRepository;
    private readonly IDatabricksRepository _databricksRepository;
    private readonly IPistaAuditoriaServices _pistaAuditoriaServices;
    private readonly System.Security.Claims.ClaimsPrincipal _currentUser;
    private readonly IRoleManagementRepository _roleManagementRepository;
    private readonly IEjecucionInfrastructure _infrastructure;

    private const string ApplicationPermissionError = "ApplicationPermissionDenied";
    private const string InternalServerError = "Error interno del servidor";
    
    public EjecucionService(
        IEjecucionRepository ejecucionRepository, 
        IJobConfigRepository jobConfigRepository, 
        IDatabricksRepository databricksRepository,
        IRoleManagementRepository roleManagementRepository,
        IPistaAuditoriaServices pistaAuditoriaServices, 
        IClaimService claimService,
        IEjecucionInfrastructure infrastructure)
    {
        _ejecucionRepository = ejecucionRepository;
        _jobConfigRepository = jobConfigRepository;
        _databricksRepository = databricksRepository;
        _pistaAuditoriaServices = pistaAuditoriaServices;
        _currentUser = claimService.GetCurrentUser();
        _roleManagementRepository = roleManagementRepository;
        _infrastructure = infrastructure;
    }

    public async Task<Result<List<EjecucionDto>>> ObtenerEjecucionesAsync()
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
                    return Result<List<EjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<List<EjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<EjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        try
        {
            var estatusList = await _ejecucionRepository.ObtenerEjecucionesAsync();
            var estatusDtoList = _infrastructure.Mapper.Map<List<EjecucionDto>>(estatusList);
            return Result<List<EjecucionDto>>.Success(estatusDtoList, "Ejecuciones obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            return Result<List<EjecucionDto>>.Failure(
                InternalServerError,
                ex.Message,
                ex.Message
            );
        }
    }

    public async Task<Result<List<ActualizarEstatusEjecucionIdResponseDto>>> ActualizarEstatusEjecucionIdAsync(ActualizarEstatusEjecucionRequestDto request)
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario }.Contains(role.RoleName))
                {
                    return Result<List<ActualizarEstatusEjecucionIdResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<List<ActualizarEstatusEjecucionIdResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<ActualizarEstatusEjecucionIdResponseDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        try
        {
            var estatusList = await _ejecucionRepository.ActualizarEstatusEjecucionIdAsync(request.EjecucionId, (int)request.EstatusEjecucion, request.Comentario);
            var result = _infrastructure.Mapper.Map<List<ActualizarEstatusEjecucionIdResponseDto>>(estatusList);
            await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = request.EjecucionId, Actividad = $"ActualizarEstatusEjecucion", Descripcion = $"Se ha actualizado el estatus de la ejecución ({request.EstatusEjecucion}) - {request.Comentario}" });
            return Result<List<ActualizarEstatusEjecucionIdResponseDto>>.Success(result, "Se ha actualizo el estatus de la ejecución.");
        }
        catch (Exception ex)
        {
            return Result<List<ActualizarEstatusEjecucionIdResponseDto>>.Failure(
                InternalServerError,
                "Error interno",
                ex.Message
            );
        }
    }

    public async Task<Result<List<EjecucionDto>>> ObtenerEjecucionPorIdAsync(int id)
    {
        try
        {
            var estatusList = await _ejecucionRepository.ObtenerEjecucionPorIdAsync(id);
            var estatusDtoList = _infrastructure.Mapper.Map<List<EjecucionDto>>(estatusList);
            await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = id, Actividad = $"ObtenerEjecucionPorId", Descripcion = $"Se obtiene una ejecución por su id: {id}" });
            return Result<List<EjecucionDto>>.Success(estatusDtoList, $"Ejecucion por id: {id} - obtenido exitosamente");
        }
        catch (Exception ex)
        {
            return Result<List<EjecucionDto>>.Failure(
                InternalServerError,
                "INTERNAL_ERROR",
                ex.Message
            );
        }
    }

    public async Task<Result<List<InsertarEjecucionDto>>> InsertarEjecucionAsync(InsertarEjecucionRequestDto ejecucion)
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario }.Contains(role.RoleName))
                {
                    return Result<List<InsertarEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<List<InsertarEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
            return Result<List<InsertarEjecucionDto>>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        try
        {
            var ejecucionEntity = _infrastructure.Mapper.Map<Ejecucion>(ejecucion);
            int nuevaEjecucionId = await _ejecucionRepository.InsertarEjecucionAsync(ejecucionEntity);
            List<InsertarEjecucionDto> lista = new List<InsertarEjecucionDto>() { new InsertarEjecucionDto() { NuevoId = nuevaEjecucionId } };
            await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = nuevaEjecucionId, Actividad = $"InsertarEjecucion", Descripcion = $"Se agregó la ejecucion exitosamente: ({ejecucion.Nombre}) - {ejecucion.IdEstatusEjecucion}" });
            return Result<List<InsertarEjecucionDto>>.Success(lista, $"Se agregó la ejecucion exitosamente");
        }
        catch (Exception ex)
        {
            return Result<List<InsertarEjecucionDto>>.Failure(
                InternalServerError,
                "INTERNAL_ERROR",
                ex.Message
            );
        }
    }

    public async Task<Result<RunJobResponseDto>> SetEjecucionEtapa(int idEjecucion, int numeroEtapa)
    {
        var roles = await _roleManagementRepository.GetAllRolesAsync();
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

        var nombreJob = "CruceInformacion";

        var job = await _jobConfigRepository.GetByNombreAsync(nombreJob);

        if (job == null)
        {
            return Result<RunJobResponseDto>.Failure($"No se encontró la configuración del trabajo '{nombreJob}'.");
        }

        var jobResultExecution = await _databricksRepository.RunJobAsync(long.Parse(job.JobId), new Dictionary<string, object>
        {
            { "etapa", numeroEtapa },
            { "id_ejecucion", idEjecucion },
            { "rfc", _currentUser.GetRFC() },
            { "ip", _infrastructure.AddressClientService.ObtenerIPCliente() }
         });
        if (jobResultExecution == null)
        {
            return Result<RunJobResponseDto>.Failure("Error al ejecutar el trabajo '{nombreJob}'.");
        }

        await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = 0, Actividad = $"SetEjecucionEtapa", Descripcion = $"ejecutar set el trabajo '{nombreJob}'. Configuración del trabajo '{nombreJob}'" });

        return Result<RunJobResponseDto>.Success(new RunJobResponseDto { NumberInJob = jobResultExecution.NumberInJob, RunId = jobResultExecution.RunId });
    }

    public async Task<Result<IntersectProcessStatusResponseDto>> CruceInformacionEstatus(IntersectProcessStatusRequestDto request)
    {
         var roles = await _roleManagementRepository.GetAllRolesAsync();
        var currentRole = _currentUser.GetRoles();
        if (_currentUser.HasAnyRole(currentRole.ToArray()))
        {
            var rolNames = roles.Select(p => p.RoleTechnician).ToList();
            if (rolNames.Any(role => currentRole.Contains(role)))
            {
                var role = roles.FirstOrDefault(r => currentRole.Contains(r.RoleTechnician));
                if (role == null || !new string[] { RolesUsuario.Administrador, RolesUsuario.Usuario}.Contains(role.RoleName))
                {
                    return Result<IntersectProcessStatusResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
                }
            }
            else
            {
                return Result<IntersectProcessStatusResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
            }
        }
        else
        {
           return Result<IntersectProcessStatusResponseDto>.Failure(_infrastructure.Translator[ApplicationPermissionError]);
        }

        try
        {
            var requestEntity = _infrastructure.Mapper.Map<IntersectProcessStatusRequest>(request);
            var responseEntity = await _ejecucionRepository.CruceInformacionEstatus(requestEntity);
            if (responseEntity == null)
            {
                return Result<IntersectProcessStatusResponseDto>.Failure("No se encontró información para el cruce de estatus.");
            }
            var responseDto = _infrastructure.Mapper.Map<IntersectProcessStatusResponseDto>(responseEntity);

            await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = request.IdEjecucion, Actividad = $"CruceInformacionEstatus", Descripcion = $"Cruce de información estatus, Etapa: {request.IdEtapa}" });

            return Result<IntersectProcessStatusResponseDto>.Success(responseDto, "Cruce de información de estatus realizado exitosamente.");
        }
        catch (Exception ex)
        {
            return Result<IntersectProcessStatusResponseDto>.Failure(
                InternalServerError,
                "INTERNAL_ERROR",
                ex.Message
            );
        }
    }

    public async Task<Result<bool>> MoverProductoEtapa(int etapa)
    {
        var job = await _jobConfigRepository.GetByNombreAsync("MoverProductoEtapa1");
        if (job == null)
        {
            return Result<bool>.Failure($"No se encontró la configuración del trabajo 'MoverProductoEtapa{etapa}'.");
        }

        // Ejecutar el trabajo en Databricks
        // Aquí asumimos que el JobId es un string que representa un número largo
        var jobResultExecution = await _databricksRepository.RunJobAsync(long.Parse(job.JobId), new Dictionary<string, object> { { "etapa", etapa } });
        if (jobResultExecution == null)
        {
            return Result<bool>.Failure($"Error al ejecutar el trabajo 'MoverProductoEtapa{etapa}'.");
        }
        await _pistaAuditoriaServices.InsertarPistaAuditoriaAsync(new InsertarPistaAuditoriaDto() { EjecucionId = 0, Actividad = $"MoverProductoEtapa{etapa}", Descripcion = $"ejecutar set el trabajo MoverProductoEtapa{etapa}. Configuración del trabajo 'MoverProductoEtapa{etapa}'" });
        return Result<bool>.Success(true, $"El trabajo 'MoverProductoEtapa{etapa}' se ejecutó exitosamente.");
    }
}
