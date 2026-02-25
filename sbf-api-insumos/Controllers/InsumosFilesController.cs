using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.DTOs;
using SAT_API.Application.Interfaces.Databricks;

namespace SAT_API.Api.Controllers;

[ApiController]
[Route("api/Insumos")]
[Authorize]
public class InsumosFilesController : ControllerBase
{
    private readonly IDatabricksService _databricksService;

    public InsumosFilesController(IDatabricksService databricksService)
    {
        _databricksService = databricksService;
    }

    [HttpGet("Download")]
    public async Task<FileContentResult> DownloadFile([FromRoute] DownloadFileStageRequestDto request)
    {
        var filePath = await _databricksService.GetDbfsFilePathByStageAsync(request);

        if (!filePath.IsSuccess || string.IsNullOrEmpty(filePath.Value))
        {
            throw new ArgumentException($"{filePath.Value} Error al obtener la ruta del archivo desde DBFS.");
        }

        var result = await _databricksService.DownloadFileFromDbfsAsync(filePath.Value);

        if (result.IsSuccess && result.Value != null && result.Value.FileContent != null)
        {
            var fileContentResult = new FileContentResult(result.Value.FileContent, result.Value.ContentType)
            {
                FileDownloadName = result.Value.FileName
            };
            return fileContentResult;
        }

        throw new ArgumentException("Error al descargar el archivo desde DBFS.");
    }
}
