using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs;
using SAT_API.Application.DTOs.Insumos;
using SAT_API.Application.Interfaces;
using SAT_API.Application.Middlewares;
using SAT_API.Domain.Common;
namespace SAT_API.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InsumosController : ControllerBase
{
    private readonly IInsumoService _insumoService;

    public InsumosController(IInsumoService insumoService)
    {
        _insumoService = insumoService;
    }

    /// <summary>
    /// Obtiene los insumos en databricks - en la Etapa 1
    /// </summary>
    /// <param name="ejecucionId">Es el id de ejecución o proceso</param>
    /// <param name="numeroEtapa">Es el numero de etapa, ejemplo: 1 = Etapa1</param>
    /// <returns>Los nombres de los archivos o insumos</returns>
    /// <remarks>
    /// 
    /// Ejemplo de solicitud:
    /// 
    /// **/api/Insumos/Etapa1/123**
    /// - 123: El id de ejecución o proceso, se relaciona con los insumos que se han cargado en la Etapa 1.
    /// 
    /// 
    /// 
    /// Ejemplo de respuesta exitoso:     
    ///     
    ///             {
    ///               "success": true,
    ///               "message": "Insumos obtenidos exitosamente",
    ///               "data": [
    ///                 {
    ///                   "archivo": "PAGO_DOMICILIO_PRAXIS1.txt"
    ///                 },
    ///                 {
    ///                   "archivo": "PAGO_DOMICILIO_PRAXIS2.txt"
    ///                 },
    ///                 {
    ///                   "archivo": "PAGO_DOMICILIO_PRAXIS3.txt"
    ///                 }
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
    /// <response code="200">Devuelve los archivos o insumos</response>
    /// <response code="404">Si no se encuentran insumos</response>
    /// <response code="500">Si ocurre un error interno del servidor</response>    
    [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status400BadRequest)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpGet("Generar/{ejecucionId}/{numeroEtapa}")]
    public async Task<ActionResult<ApiResponse<RunJobResponseDto>>> GetExistenciaInsumos([FromRoute] int ejecucionId, int numeroEtapa)
    {
        var result = await _insumoService.GenerateStageResources(ejecucionId, numeroEtapa);

        if (result.IsSuccess)
            return Ok(ApiResponse<RunJobResponseDto>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<List<InsumoDto>>.ErrorResponse(result.Message, result.Errors));
    }

    /// <summary>
    /// Obtiene los insumos de entrada en databricks - en la Etapa 1
    /// </summary>
    /// <param name="ejecucionId">Es el id de ejecución o proceso</param>
    /// <param name="numeroEtapa">Es el numero de etapa, ejemplo: 1 = Etapa1</param>
    /// <returns>Los insumos de entrada</returns>
    /// <remarks>
    /// Ejemplo de solicitud:
    /// 
    /// **/api/Insumos/Entrada/123**
    /// - 123: El id de ejecución o proceso, se relaciona con los insumos que se han cargado en la Etapa 1.
    /// Ejemplo de respuesta exitoso:         
    ///     
    ///             {
    ///                 "success": true,
    ///                "message": "Insumos obtenidos exitosamente",
    ///                "data": [
    ///                    {
    ///                        "archivo": "PAGO_DOMICILIO_PRAXIS1.txt"
    ///               },
    ///                   {
    ///                       "archivo": "PAGO_DOMICILIO_PRAXIS2.txt"
    ///               },
    ///                   {
    ///                      "archivo": "PAGO_DOMICILIO_PRAXIS3.txt"
    ///              }
    ///               ],
    ///               "errors": []
    ///           }
    ///       }
    /// Ejemplo de respuesta con error:
    ///  
    ///           {
    ///                "success": false,
    ///               "message": "Error interno del servidor",
    ///              "data": null,
    ///             "errors": [
    ///                    {
    ///                       "code": "INTERNAL_ERROR",
    ///                   "message": "No job configuration found for nombre: Get_Insumos_Etapa1X (Parameter 'jobConfig')"
    ///                   }
    ///              ]
    ///          }
    ///  </remarks>
    /// <response code="200">Devuelve los insumos de entrada</response>
    /// <response code="400">Si hay error al procesar los insumos de entrada</response>
    /// <response code="404">Si no se encuentran insumos de entrada</response>
    /// <response code="500">Si ocurre un error interno del servidor</response>
    [HttpGet("Consultar/{ejecucionId}/{numeroEtapa}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<List<InsumoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<InsumoDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<List<InsumoDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<InsumoDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<InsumoDto>>>> GetOutputResourcesAsync([FromRoute] int ejecucionId, int numeroEtapa)
    {
        var result = await _insumoService.GetOutputResourcesAsync(ejecucionId, numeroEtapa);

        if (result.IsSuccess)
            return Ok(ApiResponse<List<InsumoDto>>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<List<InsumoDto>>.ErrorResponse(result.Message, result.Errors));
    }

    /// <summary>
    /// Genera el proceso de validación los nombres en databricks - en la Etapa 1
    /// </summary>
    /// <param name="ejecucionId">Es el id de ejecución o proceso</param>
    /// <param name="numeroEtapa">Es el numero de etapa, ejemplo: 1 = Etapa1</param>
    /// <returns>El estatus del proceso de validación</returns>
    /// <remarks>
    /// 
    /// Ejemplo de solicitud:
    /// 
    /// **/api/Insumos/Validar/123**
    /// - 123: El id de ejecución o proceso, se relaciona con los insumos que se han cargado en la Etapa 1.
    /// 
    /// 
    /// Ejemplo de respuesta exitoso:         
    ///     
    ///             {
    ///                 "success": true,
    ///                 "message": "Insumos obtenidos exitosamente",
    ///                 "data": [
    ///                     {
    ///                         "runId": "850339516689349",
    ///                         "numberInJob": 703200627405335
    ///                     }
    ///                 ],
    ///                 "errors": []
    ///             }
    ///     
    /// 
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
    ///                         "message": "No job configuration found for nombre: ValidarLayout (Parameter 'jobConfig')"
    ///                     }
    ///                 ]
    ///             } 
    /// 
    /// </remarks>
    /// <response code="200">Devuelve la información con el estatus del proceso de validación</response>
    /// <response code="400">Si hay error al procesar la validación</response>
    /// <response code="404">Si no se encuentra el proceso de validación</response>
    /// <response code="500">Si ocurre un error interno del servidor</response>  
    [HttpGet("Validar/{ejecucionId}/{numeroEtapa}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<ApiResponse<ProcesoValidacionDto>>> GetValidarInsumos([FromRoute] int ejecucionId, int numeroEtapa)
    {
        var result = await _insumoService.GetValidarInsumos(ejecucionId, numeroEtapa);

        if (result.IsSuccess)
            return Ok(ApiResponse<ProcesoValidacionDto>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<ProcesoValidacionDto>.ErrorResponse(result.Message, result.Errors));

    }

    /// <summary>
    /// Obtiene el estatus de validación de un grupo de insumos específicos por su ejecucionId
    /// </summary>
    /// <param name="ejecucionId">Es el id de ejecución o proceso</param>
    /// <param name="numeroEtapa">Es el numero de etapa, ejemplo: 1 = Etapa1</param>
    /// <returns>Información del estatus de validación de los insumos solicitados</returns>
    /// <remarks>
    /// 
    /// Ejemplo de solicitud:
    /// 
    /// **/api/Insumos/EstatusValidacion/123**
    /// - 123: El id de ejecución o proceso, se relaciona con los insumos que se han cargado en la Etapa 1.
    /// 
    /// 
    /// Ejemplo de respuesta exitoso:         
    ///     
    ///              {
    ///                  "success": true,
    ///                  "message": "Estatus obtenidos exitosamente",
    ///                  "data": [
    ///                      {
    ///                          "archivo": "PAGO_DOMICILIO_PRAXIS1.txt",
    ///                          "valido": true
    ///                      },
    ///                      {
    ///                          "archivo": "PAGO_DOMICILIO_PRAXIS2.txt",
    ///                          "valido": true
    ///                      },
    ///                      {
    ///                          "archivo": "PAGO_DOMICILIO_PRAXIS3.txt",
    ///                          "valido": false
    ///                      }
    ///                  ],
    ///                  "errors": []
    ///              }
    ///     
    /// 
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
    ///                         "message": "No job configuration found for nombre: ValidarLayout (Parameter 'jobConfig')"
    ///                     }
    ///                 ]
    ///             } 
    ///             
    /// 
    /// </remarks>
    /// <response code="200">Devuelve el estatus de validación de los insumos</response>
    /// <response code="400">Si el runId es inválido o la solicitud es incorrecta</response>
    /// <response code="404">Si el runId no se encuentra</response>
    /// <response code="500">Si ocurre un error interno del servidor</response>
    [HttpGet("Validados/{ejecucionId}/{numeroEtapa}")]
    [ProducesResponseType(typeof(EstatusValidacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<EstatusValidacionDto>>> GetEstatusValidacion([FromRoute] int ejecucionId, int numeroEtapa)
    {
        var result = await _insumoService.GetEstatusValidacionAsync(ejecucionId, numeroEtapa);

        if (result.IsSuccess)
            return Ok(ApiResponse<List<EstatusValidacionDto>>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<List<EstatusValidacionDto>>.ErrorResponse(result.Message, result.Errors));

    }

    /// <summary>
    /// Obtiene el estatus del trabajo de recursos en databricks
    /// </summary>
    /// <param name="runId">Es el id del trabajo de recursos</param>
    /// <returns>El estatus del trabajo de recursos</returns>
    /// <remarks>
    /// 
    /// Ejemplo de solicitud:
    /// 
    /// **/api/Insumos/EstatusTrabajoRecursos/12345**
    /// - 12345: El id del trabajo de recursos, se relaciona con el trabajo que se ha generado en la Etapa 1.
    /// 
    /// 
    /// Ejemplo de respuesta exitoso:
    ///     
    ///             {
    ///                 "success": true,
    ///                 "message": "Estatus del trabajo de recursos obtenido exitosamente",
    ///                 "data": "COMPLETED",
    ///                 "errors": []
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
    /// <response code="200">Devuelve el estatus del trabajo de recursos</response>
    /// <response code="400">Si el runId es inválido o la solicitud es incorrecta</response>
    /// <response code="404">Si el runId no se encuentra</response>
    /// <response code="500">Si ocurre un error interno del servidor</response>
    [HttpGet("JobStatus/{runId}")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RunStatusJobResponseDto>>> StatusJobResourceAsync([FromRoute] long runId)
    {
        var result = await _insumoService.StatusJobResourceAsync(runId);

        if (result.IsSuccess)
            return Ok(ApiResponse<RunStatusJobResponseDto>.SuccessResponse(result.Value!, result.Message));

        return Ok(ApiResponse<RunStatusJobResponseDto>.ErrorResponse(result.Message, result.Errors));
    }
}