namespace SAT_API.Domain.Entities.Track;

public class DataPopulationValidationResponse
{
    [ColumnMap("estatus")]
    public string Status { get; set; } = string.Empty;
    [ColumnMap("estatus_valor")]
    public int Value { get; set; }
}
