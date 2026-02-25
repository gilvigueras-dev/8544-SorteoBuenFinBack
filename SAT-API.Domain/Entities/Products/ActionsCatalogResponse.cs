namespace SAT_API.Domain.Entities.Products
{
    public class ActionsCatalogResponse
    {
        public int Id { get; set; }
        public string Apartado { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}