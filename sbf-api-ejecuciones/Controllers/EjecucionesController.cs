using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Common;
using SAT_API.Application.DTOs.Ejecuciones;
using SAT_API.Application.DTOs.Process;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Common;

namespace SAT_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EjecucionesController : ControllerBase
    {
        private readonly IEjecucionService _ejecucionService;
        private readonly IMapper _mapper;

        public EjecucionesController(IEjecucionService ejecucionService, IMapper mapper)
        {
            _ejecucionService = ejecucionService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene la lista de ejecuciones
        /// </summary>
        /// <returns>Lista de ejecuciones</returns>
        /// <remarks>
        /// 
        /// Ejemplo de solicitud:
        /// 
        /// **/api/Ejecuciones/Obtener**
        /// 
        /// 
        /// Ejemplo de respuesta exitoso:     
        ///     
        ///             {
        ///               "success": true,
        ///               "message": "Ejecuciones obtenidos exitosamente",
        ///               "data": 
        ///               [
        ///                     {
        ///                       "id": 1,
        ///                       "nombre": "EJECUCION 20250617_231750",
        ///                       "fecha": "2025-06-17T23:17:50.313127",
        ///                       "idEstatusEjecucion": 0,
        ///                       "estatusEjecucion": "Nuevo",
        ///                       "estatus": "En proceso"
        ///                     },
        ///                     {
        ///                       "id": 2,
        ///                       "nombre": "EJECUCION 20250617_231614",
        ///                       "fecha": "2025-06-17T23:16:14.11871",
        ///                       "idEstatusEjecucion": 0,
        ///                       "estatusEjecucion": "Nuevo",
        ///                       "estatus": "En proceso"
        ///                     },
        ///               ],
        ///               "errors": []
        ///             }
        ///     
        /// Ejemplo de respuesta con error:     
        /// 
        ///             {
        ///                 "success": false,
        ///                 "message": "Error interno del servidor",
        ///                 "data": null,
        ///                 "errors": [
        ///                     {
        ///                         "code": "INTERNAL_ERROR",
        ///                         "message": "No job configuration found for nombre: Get_Insumos_Etapa1X (Parameter 'jobConfig')"
        ///                     }
        ///                 ]
        ///             }
        /// 
        /// </remarks>
        /// <response code="200">Devuelve la lista</response>
        /// <response code="404">Si no se encuentran</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>    
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet("Obtener")]
        public async Task<ActionResult<ApiResponse<List<EjecucionDto>>>> ObtenerEjecucionesAsync()
        {
            var result = await _ejecucionService.ObtenerEjecucionesAsync();

            if (result.IsSuccess)
                return Ok(ApiResponse<List<EjecucionDto>>.SuccessResponse(result.Value!, result.Message));

            return Ok(ApiResponse<List<EjecucionDto>>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Obtiene la lista de ejecuciones
        /// </summary>
        /// <param name="id">Id de la ejecución</param>
        /// <returns>El registro de la ejecución</returns>
        /// <remarks>
        /// 
        /// Ejemplo de solicitud:
        /// 
        /// **/api/Ejecuciones/Obtener/123**
        /// -123 es el ID de la ejecución que se desea obtener.
        /// 
        /// Ejemplo de respuesta exitoso:     
        ///     
        ///             {
        ///               "success": true,
        ///               "message": "Ejecuciones obtenidos exitosamente",
        ///               "data": 
        ///               [
        ///                     {
        ///                       "id": 1,
        ///                       "nombre": "EJECUCION 20250617_231750",
        ///                       "fecha": "2025-06-17T23:17:50.313127",
        ///                       "idEstatusEjecucion": 0,
        ///                       "estatusEjecucion": "Nuevo",
        ///                       "estatus": "En proceso"
        ///                     }
        ///               ],
        ///               "errors": []
        ///             }
        ///     
        /// Ejemplo de respuesta con error:     
        /// 
        ///             {
        ///                 "success": false,
        ///                 "message": "Error interno del servidor",
        ///                 "data": null,
        ///                 "errors": [
        ///                     {
        ///                         "code": "INTERNAL_ERROR",
        ///                         "message": "No job configuration found for nombre: Get_Insumos_Etapa1X (Parameter 'jobConfig')"
        ///                     }
        ///                 ]
        ///             }
        /// 
        /// </remarks>
        /// <response code="200">Devuelve la lista</response>
        /// <response code="404">Si no se encuentran</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>    
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet("Obtener/{id}")]
        public async Task<ActionResult<ApiResponse<List<EjecucionDto>>>> ObtenerEjecucionesAsync(int id)
        {
            var result = await _ejecucionService.ObtenerEjecucionPorIdAsync(id);

            if (result.IsSuccess)
                return Ok(ApiResponse<List<EjecucionDto>>.SuccessResponse(result.Value!, result.Message));

            return Ok(ApiResponse<List<EjecucionDto>>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Agrega un nuevo registro de ejecución
        /// </summary>
        /// <returns>El id de la nueva ejecución</returns>
        /// <remarks>
        /// 
        /// Ejemplo de solicitud:
        /// 
        /// **/api/Ejecuciones/Insertar**
        /// 
        /// Body de ejemplo
        /// 
        ///             {
        ///               "nombre": "Prueba",
        ///               "fecha": "2025-06-20T23:47:49.565Z",
        ///               "idEstatusEjecucion": 2
        ///             } 
        /// 
        /// 
        /// Ejemplo de respuesta exitoso:     
        ///     
        ///             {
        ///               "success": true,
        ///               "message": "Ejecuciones obtenidos exitosamente",
        ///               "data": 
        ///               [
        ///                     {
        ///                       "id": 1,
        ///                       "nombre": "EJECUCION 20250617_231750",
        ///                       "fecha": "2025-06-17T23:17:50.313127",
        ///                       "idEstatusEjecucion": 0,
        ///                       "estatusEjecucion": "Nuevo",
        ///                       "estatus": "En proceso"
        ///                     }
        ///               ],
        ///               "errors": []
        ///             }
        ///     
        /// Ejemplo de respuesta con error:     
        /// 
        ///             {
        ///                 "success": false,
        ///                 "message": "Error interno del servidor",
        ///                 "data": null,
        ///                 "errors": [
        ///                     {
        ///                         "code": "INTERNAL_ERROR",
        ///                         "message": "No job configuration found for nombre: Get_Insumos_Etapa1X (Parameter 'jobConfig')"
        ///                     }
        ///                 ]
        ///             }
        /// 
        /// </remarks>
        /// <response code="200">Devuelve la lista</response>
        /// <response code="404">Si no se encuentran</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<EjecucionDto>>), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost("Insertar")]
        public async Task<ActionResult<ApiResponse<List<InsertarEjecucionDto>>>> InsertarEjecucionesAsync([FromBody] InsertarEjecucionRequestDto request)
        {
            var result = await _ejecucionService.InsertarEjecucionAsync(request);

            if (result.IsSuccess)
                return Ok(ApiResponse<List<InsertarEjecucionDto>>.SuccessResponse(result.Value!, result.Message));

            return Ok(ApiResponse<List<InsertarEjecucionDto>>.ErrorResponse(result.Message, result.Errors));
        }

        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost("SetEjecucionEtapa")]
        public async Task<ActionResult<ApiResponse<RunJobResponseDto>>> SetEjecucionEtapa([FromBody] EjecucionCruceEtapaDto request)
        {
            var result = await _ejecucionService.SetEjecucionEtapa(request.IdEjecucion, request.NumeroEtapa);

            if (result.IsSuccess)
                return Ok(ApiResponse<RunJobResponseDto>.SuccessResponse(result.Value!, result.Message));

            return Ok(ApiResponse<RunJobResponseDto>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Obtiene el estatus del proceso de cruce de información
        /// Obtiene el estatus del proceso de cruce de información
        /// </summary>
        /// <param name="request">Solicitud con los parámetros necesarios</param>
        /// <returns>El estatus del proceso de cruce de información</returns>
        /// <remarks>
        /// 
        /// Ejemplo de solicitud:
        /// **/api/Ejecuciones/CruceInformacionEstatus/123/1**
        /// - 123 es el ID de la ejecución y 1 es el ID de la etapa. 
        /// Ejemplo de respuesta exitoso:     
        ///   
        /// **/api/Ejecuciones/CruceInformacionEstatus/123/1**
        /// - 123 es el ID de la ejecución y 1 es el ID de la etapa. 
        /// Ejemplo de respuesta exitoso:     
        ///   
        ///             {
        ///               "success": true,      
        ///              "message": "Estatus del proceso de cruce de información obtenido exitosamente.",
        ///              "data": {
        ///                   "idEjecucion": 123,
        ///                  "idEtapa": 1,
        ///                  "estatus": "En proceso",
        ///                  "mensaje": "El proceso de cruce de información está en ejecución."
        ///              },
        ///               "success": true,      
        ///              "message": "Estatus del proceso de cruce de información obtenido exitosamente.",
        ///              "data": {
        ///                   "idEjecucion": 123,
        ///                  "idEtapa": 1,
        ///                  "estatus": "En proceso",
        ///                  "mensaje": "El proceso de cruce de información está en ejecución."
        ///              },
        ///              "errors": []
        ///             }
        ///             }
        /// Ejemplo de respuesta con error:
        ///             {
        ///            "success": false,
        ///           "message": "Error al obtener el estatus del proceso de cruce de información.",
        ///           "data": null,
        ///          "errors": [
        ///                {
        ///                   "code": "INTERNAL_ERROR",
        ///                  "message": "No se encontró la configuración del trabajo 'CruceInformacionEstatus'."
        ///               }
        ///          ]
        ///    }
        ///             {
        ///            "success": false,
        ///           "message": "Error al obtener el estatus del proceso de cruce de información.",
        ///           "data": null,
        ///          "errors": [
        ///                {
        ///                   "code": "INTERNAL_ERROR",
        ///                  "message": "No se encontró la configuración del trabajo 'CruceInformacionEstatus'."
        ///               }
        ///          ]
        ///    }
        /// </remarks>
        [ProducesResponseType(typeof(ApiResponse<IntersectProcessStatusResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IntersectProcessStatusResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<IntersectProcessStatusResponseDto>), StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet("CruceInformacionEstatus")]
        public async Task<ActionResult<ApiResponse<IntersectProcessStatusResponseDto>>> CruceInformacionEstatus([FromRoute] IntersectProcessStatusRequestDto request)
        {
            var result = await _ejecucionService.CruceInformacionEstatus(request);
            if (result.IsSuccess)
                return Ok(ApiResponse<IntersectProcessStatusResponseDto>.SuccessResponse(result.Value!, result.Message));
            return Ok(ApiResponse<IntersectProcessStatusResponseDto>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Cancela una ejecución específica
        /// Cancela una ejecución específica
        /// </summary>
        /// <returns>El id de la ejecución</returns>
        /// <remarks>
        /// 
        /// Ejemplo de solicitud:
        /// 
        /// **/api/Ejecuciones/Cancelar**
        /// 
        /// Body de ejemplo
        /// 
        ///             {
        ///               "ejecucionId": 30,
        ///               "comentario": "La razón de la cancelación"
        ///             } 
        /// 
        /// 
        /// Ejemplo de respuesta exitoso:     
        ///     
        ///             {
        ///               "success": true,
        ///               "message": "Ejecuciones obtenidos exitosamente",
        ///               "data": 
        ///               [
        ///                     {
        ///                       "id": 1
        ///                     }
        ///               ],
        ///               "errors": []
        ///             }
        ///     
        /// Ejemplo de respuesta con error:     
        /// 
        ///             {
        ///                 "success": false,
        ///                 "message": "Error interno del servidor",
        ///                 "data": null,
        ///                 "errors": [
        ///                     {
        ///                         "code": "INTERNAL_ERROR",
        ///                         "message": "No job configuration found for nombre: Get_Insumos_Etapa1X (Parameter 'jobConfig')"
        ///                     }
        ///                 ]
        ///             }
        /// 
        /// </remarks>
        /// <response code="200">Devuelve la lista</response>
        /// <response code="404">Si no se encuentran</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>
        [HttpPatch("Cancelar")]
        [ProducesResponseType(typeof(ApiResponse<ActualizarEstatusEjecucionIdResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ActualizarEstatusEjecucionIdResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<ActualizarEstatusEjecucionIdResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<ActualizarEstatusEjecucionIdResponseDto>>>> CancelarEjecucionAsync([FromBody] CancelarEjecucionRequestDto request)
        {
            ActualizarEstatusEjecucionRequestDto newRequest = _mapper.Map<ActualizarEstatusEjecucionRequestDto>(request);
            var result = await _ejecucionService.ActualizarEstatusEjecucionIdAsync(newRequest);

            if (result.IsSuccess)
                return Ok(ApiResponse<List<ActualizarEstatusEjecucionIdResponseDto>>.SuccessResponse(result.Value!, result.Message));

            return Ok(ApiResponse<List<ActualizarEstatusEjecucionIdResponseDto>>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Actualiza el estatus de una ejecución específica
        /// </summary>
        /// <returns>El id de la ejecución</returns>
        /// <remarks>
        /// 
        /// Ejemplo de solicitud:
        /// 
        /// **/api/Ejecuciones/Cancelar**
        /// 
        /// Body de ejemplo
        /// 
        ///             {
        ///               "ejecucionId": 30,
        ///               "comentario": "La razón de la cancelación"
        ///             } 
        /// 
        /// 
        /// Ejemplo de respuesta exitoso:     
        ///     
        ///             {
        ///               "success": true,
        ///               "message": "Ejecuciones obtenidos exitosamente",
        ///               "data": 
        ///               [
        ///                     {
        ///                       "id": 1
        ///                     }
        ///               ],
        ///               "errors": []
        ///             }
        ///     
        /// Ejemplo de respuesta con error:     
        /// 
        ///             {
        ///                 "success": false,
        ///                 "message": "Error interno del servidor",
        ///                 "data": null,
        ///                 "errors": [
        ///                     {
        ///                         "code": "INTERNAL_ERROR",
        ///                         "message": "No job configuration found for nombre: Get_Insumos_Etapa1X (Parameter 'jobConfig')"
        ///                     }
        ///                 ]
        ///             }
        /// 
        /// </remarks>
        /// <response code="200">Devuelve la lista</response>
        /// <response code="404">Si no se encuentran</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>
        [HttpPatch("Actualizar")]
        [ProducesResponseType(typeof(ApiResponse<ActualizarEstatusEjecucionIdResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ActualizarEstatusEjecucionIdResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<ActualizarEstatusEjecucionIdResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<ActualizarEstatusEjecucionIdResponseDto>>>> ActualizarEjecucionAsync([FromBody] ActualizarEstatusEjecucionRequestDto request)
        {
            var result = await _ejecucionService.ActualizarEstatusEjecucionIdAsync(request);

            if (result.IsSuccess)
                return Ok(ApiResponse<List<ActualizarEstatusEjecucionIdResponseDto>>.SuccessResponse(result.Value!, result.Message));

            return Ok(ApiResponse<List<ActualizarEstatusEjecucionIdResponseDto>>.ErrorResponse(result.Message, result.Errors));
        }


        /// <summary>
        /// Mueve el producto a la etapa indicada
        /// </summary>
        /// <returns>Mensaje de éxito o error</returns>
        /// <remarks>
        /// 
        /// El parámetro request es una estructura 
        /// {
        ///     stageNumber:int
        /// }
        /// donde stageNumber es el **número de la etapa a mover prouctos**
        /// 
        /// Ejemplo de solicitud:
        /// 
        /// **/api/Ejecuciones/MoverProductoEtapa**
        /// 
        /// 
        /// Ejemplo de respuesta exitoso:
        ///
        ///             {
        ///               "success": true,
        ///               "message": "Producto movido a la etapa exitosamente.",
        ///               "data": "Producto movido a la etapa exitosamente.",
        ///               "errors": []
        ///             }
        /// 
        /// Ejemplo de respuesta con error:
        /// 
        ///             {
        ///                 "success": false,
        ///                 "message": "Error al mover el producto a la etapa.",
        ///                 "data": null,
        ///                 "errors": [                 
        ///                    {
        ///                        "code": "INTERNAL_ERROR",
        ///                       "message": "No se encontró la configuración del trabajo 'MoverProductoEtapa'."
        ///                    }
        ///                 ]
        ///            }
        ///</remarks>
        /// <response code="200">Devuelve el mensaje de éxito</response>
        /// <response code="404">Si no se encuentra la configuración del trabajo</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost("MoverProductoEtapa")]
        public async Task<ActionResult<ApiResponse<bool>>> MoveProductByStage([FromBody] StageGeneralRequestDto request)
        {
            var result = await _ejecucionService.MoverProductoEtapa(request.StageNumber);
            if (result.IsSuccess)
                return Ok(ApiResponse<bool>.SuccessResponse(result.Value!, result.Message));
            return Ok(ApiResponse<bool>.ErrorResponse(result.Message, result.Errors));
        }

    }
}