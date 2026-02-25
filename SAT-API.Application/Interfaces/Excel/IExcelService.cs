using SAT_API.Application.DTOs.Excel;
using SAT_API.Domain.Entities.Products;

namespace SAT_API.Application.Interfaces.Excel;

public interface IExcelService
{
    Task<byte[]> GenerarExcelProductos(List<AuditTrailResponse> productos, ExcelExportRequestDto? request = null);

    Task<byte[]> GenerarExcelGenerico<T>(List<T> datos, string nombreHoja = "Datos", ExcelExportRequestDto? request = null);
}