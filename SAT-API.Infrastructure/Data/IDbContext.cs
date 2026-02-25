using System.Data;

namespace SAT_API.Infrastructure.Data
{
    public interface IDbContext
    {
        IDbConnection CreateConnection();
        
    }
}
