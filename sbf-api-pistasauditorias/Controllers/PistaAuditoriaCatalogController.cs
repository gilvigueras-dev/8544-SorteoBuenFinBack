using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs.PistasAuditorias;
using SAT_API.Application.DTOs.Products;
using SAT_API.Application.Interfaces;
using SAT_API.Domain.Common;
using SAT_API.Domain.Entities.Products;

namespace SAT_API.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/PistaAuditoria")]
public class PistaAuditoriaCatalogController : ControllerBase
{
    private readonly IPistaAuditoriaServices _pistaAuditoriaServices;

    public PistaAuditoriaCatalogController(IPistaAuditoriaServices pistaAuditoriaServices)
    {
        _pistaAuditoriaServices = pistaAuditoriaServices;
    }

    [HttpGet("excel")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<byte[]>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<byte[]>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportarProductosExcelGet([FromQuery] ProductsExcelExportRequestDto request)
    {
        try
        {
            var result = await _pistaAuditoriaServices.GetAuditTrailsAsync(request);
            var fileName = $"PistasAuditoria_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (result.Value == null)
            {
                return BadRequest("No se encontraron datos para exportar.");
            }

            return File(result.Value, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al generar el archivo CSV: {ex.Message}");
        }
    }

    [HttpGet("catalog/GetActions")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ActionsCatalogResponse>>> GetActionsCatalogAsync()
    {
        var result = await _pistaAuditoriaServices.GetActionsCatalogAsync();

        if (result.IsSuccess)
            return Ok(ApiResponse<List<ActionsCatalogResponse>>.SuccessResponse(result.Value ?? new List<ActionsCatalogResponse>(), result.Message));

        return Ok(ApiResponse<List<ActionsCatalogResponse>>.ErrorResponse(result.Message, result.Errors));
    }

    [HttpGet("Catalog/GetStages")]
    [ProducesResponseType(typeof(ApiResponse<List<StageCatalogResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<StageCatalogResponseDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<List<StageCatalogResponseDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<StageCatalogResponseDto>>>> GetStagesCatalogAsync()
    {
        var result = await _pistaAuditoriaServices.GetStagesCatalogAsync();

        if (result.IsSuccess)
            return Ok(ApiResponse<List<StageCatalogResponseDto>>.SuccessResponse(result.Value ?? new List<StageCatalogResponseDto>(), result.Message));

        return Ok(ApiResponse<List<StageCatalogResponseDto>>.ErrorResponse(result.Message, result.Errors));
    }
}
