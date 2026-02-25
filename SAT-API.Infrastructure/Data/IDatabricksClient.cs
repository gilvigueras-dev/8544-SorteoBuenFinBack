namespace SAT_API.Infrastructure.Data
{
    public interface IDatabricksClient
    {
        HttpClient GetClient();
    }
}
