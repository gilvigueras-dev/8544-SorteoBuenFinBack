using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs;
using SAT_API.Application.Interfaces.Databricks;
using SAT_API.Domain.Common;

namespace SAT_API.Api.Controllers
{
    [Route("api/Ejecuciones")]
    [ApiController]
    [Authorize]
    public class EjecucionesJobsController : ControllerBase
    {
        private readonly IDatabricksService _databricksService;

        public EjecucionesJobsController(IDatabricksService databricksService)
        {
            _databricksService = databricksService;
        }

        /// <summary>
        /// Ejecuta un trabajo en Databricks
        /// </summary>
        /// <param name="request">Solicitud con el nombre del trabajo y parámetros</param>
        /// <returns>Respuesta del trabajo ejecutado</returns>
        /// <remarks>
        /// 
        /// Ejemplo de solicitud:
        /// **/api/Ejecuciones/RunJob**
        /// 
        /// Body de ejemplo
        /// 
        ///             {
        ///               "jobName": "Get_Insumos_Etapa1X",
        ///               "parameters": {
        ///                 "param1": "value1",
        ///                 "param2": "value2"
        ///               }
        ///             }
        /// 
        /// Ejemplo de respuesta exitoso:
        /// 
        ///             {
        ///               "success": true,
        ///               "message": "Job executed successfully.",
        ///               "data": {
        ///                 "jobId": "12345",
        ///                 "runId": "67890",
        ///                 "status": "RUNNING"
        ///               },
        ///               "errors": []
        ///             }
        /// 
        /// Ejemplo de respuesta con error:
        /// 
        ///             {
        ///                 "success": false,
        ///                 "message": "Error executing job.",
        ///                 "data": null,
        ///                 "errors": [
        ///                     {
        ///                         "code": "INTERNAL_ERROR",
        ///                         "message": "No job configuration found for nombre: Get_Insumos_Etapa1X (Parameter 'jobConfig')"
        ///                     }
        ///                 ]
        ///             }
        /// </remarks>
        /// <response code="200">Devuelve la respuesta del trabajo ejecutado</response>
        /// <response code="404">Si no se encuentra la configuración del trabajo</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>
        /// <response code="400">Si la solicitud es inválida</response>
        /// <response code="401">Si no está autorizado para ejecutar el trabajo</response>
        /// <response code="403">Si el acceso está prohibido</response>
        /// <response code="409">Si hay un conflicto al ejecutar el trabajo</response>
        [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<RunJobResponseDto>), StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPost("RunJob")]
        public async Task<ActionResult<ApiResponse<RunJobResponseDto>>> RunJob([FromBody] RunJobRequestDto request)
        {
            var result = await _databricksService.RunJobAsync(request);
            if (result.IsSuccess)
                return Ok(ApiResponse<RunJobResponseDto>.SuccessResponse(result.Value!, result.Message));
            return Ok(ApiResponse<RunJobResponseDto>.ErrorResponse(result.Message, result.Errors));
        }
    }
}
