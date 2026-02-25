namespace SAT_API.Domain.Entities;

public class EstatusValidacion
{
    [ColumnMap("id_archivo")]
    public int FileId { get; set; }
    [ColumnMap("nombre")]
    public string FileName { get; set; } = string.Empty;
    [ColumnMap("existe")]
    public bool Exists { get; set; }
    [ColumnMap("validado")]
    public bool IsValidated { get; set; }
    [ColumnMap("cc_generadas")]
    public bool CCGenerated { get; set; }
    [ColumnMap("advertencia")]
    public bool Warning { get; set; }
    [ColumnMap("ruta_cc")]
    public string? CCPath { get; set; }
    [ColumnMap("descargado")]
    public bool Downloaded { get; set; }
}
