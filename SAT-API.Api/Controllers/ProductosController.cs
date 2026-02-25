using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.Interfaces.Products;
using SAT_API.Domain.Common;

namespace SAT_API.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductosController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        /// <summary>
        /// Obtiene los productos según los parámetros proporcionados.
        /// </summary>
        /// <param name="request">Parámetros de búsqueda de productos.</param>
        /// <returns>Lista de productos encontrados.</returns>
        [HttpGet("GetProductos")]
        [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetProductos([FromRoute] ProductRequestDto request)
        {
            var result = await _productsService.GetProducts(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<List<ProductResponseDto>>.SuccessResponse(result.Value!, result.Message));
            }
            return Ok(ApiResponse<List<ProductResponseDto>>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Actualiza el estatus de un producto.
        /// </summary>
        /// <param name="request">Solicitud para actualizar el estatus del producto.</param>
        /// <returns>Mensaje de éxito o error.</returns>
        [HttpPost("SetVoboProductos")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> SetProductStatus([FromBody] ProductStatusRequestDto request)
        {
            var result = await _productsService.SetProductStatus(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<string>.SuccessResponse(result.Value!, result.Message));
            }
            return Ok(ApiResponse<string>.ErrorResponse(result.Message, result.Errors));
        }

        /// <summary>
        /// Obtiene el datos del reporte de cifras de control
        /// </summary>
        /// <param name="request">Parámetros de entrada para obtener las cifras de control en base a la clas <seealso cref="ControlNumbersRequestDto"/></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResponse<List<ControlNumbersResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<ControlNumbersResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<ControlNumbersResponseDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<ControlNumbersResponseDto>>), StatusCodes.Status500InternalServerError)]
        [HttpGet("ControlNumbers")]
        public async Task<ActionResult<ApiResponse<List<ControlNumbersResponseDto>>>> ControlNumbers([FromRoute] ControlNumbersRequestDto request)
        {
            var result = await _productsService.ControlNumberList(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<List<ControlNumbersResponseDto>>.SuccessResponse(result.Value!, result.Message));
            }
            return Ok(ApiResponse<List<ControlNumbersResponseDto>>.ErrorResponse(result.Message, result.Errors));
        }


        /// <summary>
        /// Obtiene la URL de los datos de cifras de control.
        /// </summary>
        /// <param name="request">Parámetros de entrada para obtener las cifras de control en base a la clas <seealso cref="ControlNumbersRequestDto"/></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResponse<ControlNumbersUrlResponseDto?>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ControlNumbersUrlResponseDto?>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ControlNumbersUrlResponseDto?>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<ControlNumbersUrlResponseDto?>), StatusCodes.Status500InternalServerError)]
        [HttpGet("ControlNumbersUrl")]
        public async Task<ActionResult<ControlNumbersUrlResponseDto?>> ControlNumbersUrl([FromRoute] ControlNumbersRequestDto request)
        {
            var result = await _productsService.ControlNumbersUrl(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<ControlNumbersUrlResponseDto?>.SuccessResponse(result.Value, result.Message));
            }

            return Ok(ApiResponse<ControlNumbersUrlResponseDto?>.ErrorResponse(result.Message, result.Errors));
        }
        /// <summary>
        /// Genera el proceso de mover archivo en databricks - en la Etapa 1
        /// </summary>
        /// <param name="ejecucionId">Es el id de ejecución o proceso</param>
        /// <param name="numeroEtapa">Es el numero de etapa, ejemplo: 1 = Etapa1</param>
        /// <param name="productoId"> Es el identificador de producto</param>
        /// <returns>El estatus del proceso de mover archvio</returns>
        /// 
        /// <remarks>
        /// 
        /// Ejemplo de solicitud:
        /// 
        /// **/api/Productos/MoverArchivo/123**
        /// - 123: El id de ejecución o proceso.
        /// 
        /// 
        /// Ejemplo de respuesta exitoso:         
        ///     
        ///             {
        ///                 "success": true,
        ///                 "message": "Proceso ejecutado exitosamente",
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
        ///                         "message": "No job configuration found for nombre: 410907466415749 (Parameter 'jobConfig')"
        ///                     }
        ///                 ]
        ///             } 
        /// 
        /// </remarks>
        /// <response code="200">Devuelve la información con el estatus del proceso de validación</response>
        /// <response code="400">Si hay error al procesar la validación</response>
        /// <response code="404">Si no se encuentra el proceso de validación</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>  
        /// var result = await _insumoService.GetValidarInsumos(ejecucionId, numeroEtapa);
        [HttpGet("MoverArchivo/{ejecucionId}/{numeroEtapa}/{productoId}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<ApiResponse<MoveFileResponseDto>>> GetMoverArchivo([FromRoute] int ejecucionId, int numeroEtapa, int productoId)
        {
            var result = await _productsService.GetMoverArchivo(ejecucionId, numeroEtapa, productoId);

            if (result.IsSuccess)
                return Ok(ApiResponse<MoveFileResponseDto>.SuccessResponse(result.Value!, result.Message));

            return Ok(ApiResponse<MoveFileResponseDto>.ErrorResponse(result.Message, result.Errors));

        }


    }
}
