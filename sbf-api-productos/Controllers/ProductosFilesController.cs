using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAT_API.Application.Interfaces.Databricks;

namespace SAT_API.Api.Controllers
{
    [ApiController]
    [Route("api/Productos")]
    [Authorize]
    public class ProductosFilesController : ControllerBase
    {
        private readonly IDatabricksService _databricksService;

        public ProductosFilesController(IDatabricksService databricksService)
        {
            _databricksService = databricksService;
        }

        /// <summary>
        /// Descarga un archivo desde DBFS.
        /// </summary>
        /// <param name="filePath">Ruta del archivo en DBFS.</param>
        /// <returns>Contenido del archivo descargado.</returns>
        [HttpGet("DownloadFile/{*filePath}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DownloadFile([FromRoute] string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return BadRequest("Ruta del archivo inválida.");

            var contentType = "application/octet-stream";

            return new Domain.Entities.Databricks.FileCallbackResult(contentType, async (outputStream, cancellationToken) =>
            {
                await _databricksService.StreamFileFromDbfsAsync(filePath, outputStream);
            })
            {
                FileDownloadName = Path.GetFileName(Uri.UnescapeDataString(filePath.Trim())) ?? "downloaded_file",
                EnableRangeProcessing = true
            };
        }
    }
}
