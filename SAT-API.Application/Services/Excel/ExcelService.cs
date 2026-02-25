using OfficeOpenXml;
using OfficeOpenXml.Style;
using SAT_API.Application.DTOs.Excel;
using SAT_API.Application.Interfaces.Excel;
using SAT_API.Domain.Entities.Products;
using System.Text;

namespace SAT_API.Application.Services.Excel;

public class ExcelService : IExcelService
{
    public async Task<byte[]> GenerarExcelProductos(List<AuditTrailResponse> productos, ExcelExportRequestDto? request = null)
    {
        var csv = new StringBuilder();

        // Configurar encabezados
        var headers = new string[] { "IdEvento","Folio", "RFCC", "Etapa", "ID Ejecución", "Fecha de Consulta Inicio", "Fecha de Consulta Final", "Acción",
        "Rol Archivo", "Nombre Archivo", "URL", "IP/Identificador" };

        // Agregar encabezados al CSV
        csv.AppendLine(string.Join(",", headers.Select(h => EscaparCampoCSV(h))));

        // Llenar datos
        foreach (var producto in productos)
        {
            var fila = new string[]
            {
                EscaparCampoCSV(producto.EventId.ToString()),
                EscaparCampoCSV(producto.Folio),
                EscaparCampoCSV(producto.RFC),
                EscaparCampoCSV(producto.Stage.ToString()),
                EscaparCampoCSV(producto.ExecutionId.ToString()),
                EscaparCampoCSV(producto.StartQueryDate.HasValue ? producto.StartQueryDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""),
                EscaparCampoCSV(producto.EndQueryDate.HasValue ? producto.EndQueryDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""),
                EscaparCampoCSV(producto.Action),
                EscaparCampoCSV(producto.FileRole ?? ""),
                EscaparCampoCSV(producto.FileName ?? ""),
                EscaparCampoCSV(producto.Url ?? ""),
                EscaparCampoCSV(producto.IPOrIdentifier ?? "")
            };

            csv.AppendLine(string.Join(",", fila));
        }

        // Convertir a bytes usando UTF-8 con BOM para compatibilidad con Excel
        var encoding = new UTF8Encoding(true);
        return await Task.FromResult(encoding.GetBytes(csv.ToString()));
    }

    private static string EscaparCampoCSV(string campo)
    {
        if (string.IsNullOrEmpty(campo))
            return "";

        // Si contiene coma, comilla doble o salto de línea, envolver en comillas
        if (campo.Contains(',') || campo.Contains('\'') || campo.Contains('\n') || campo.Contains('\r'))
        {
            // Duplicar las comillas dobles existentes y envolver el texto en comillas
            return $"\"{campo.Replace("\"", "\"\"")}\"";
        }

        return campo;
    }

    public Task<byte[]> GenerarExcelGenerico<T>(List<T> datos, string nombreHoja = "Datos", ExcelExportRequestDto? request = null)
    {
        // Implementation for generating generic Excel file
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();

        var worksheet = package.Workbook.Worksheets.Add(nombreHoja);

        if (datos == null || datos.Count == 0)
        {
            worksheet.Cells[1, 1].Value = "No hay datos para mostrar";
            return Task.FromResult(package.GetAsByteArray());
        }

        // Obtener propiedades del tipo T
        var properties = typeof(T).GetProperties();

        // Configurar encabezados
        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = properties[i].Name;
            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        }

        // Llenar datos
        for (int i = 0; i < datos.Count; i++)
        {
            var item = datos[i];
            for (int j = 0; j < properties.Length; j++)
            {
                var value = properties[j].GetValue(item);
                worksheet.Cells[i + 2, j + 1].Value = value;
            }
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return Task.FromResult(package.GetAsByteArray());
    }
}