using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Reflection;

namespace SAT_API.Infrastructure.Data
{
    public class DbContext : IDbContext
    {
        private readonly IConfiguration _configuration;

        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var defaultConnection = _configuration.GetConnectionString("DefaultConnection");
            if (String.IsNullOrEmpty(defaultConnection))
            {
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
                {
                    Host = EnvironmentService.GetRequiredEnvironmentVariable("SBF-ENV-DB-HOST"),
                    Port = int.Parse(EnvironmentService.GetRequiredEnvironmentVariable("SBF-ENV-DB-PORT")),
                    Username = EnvironmentService.GetRequiredEnvironmentVariable("SBF-ENV-DB-USER"),
                    Password = EnvironmentService.GetRequiredEnvironmentVariable("SBF-ENV-DB-PASSWORD"),
                    Database = EnvironmentService.GetRequiredEnvironmentVariable("SBF-ENV-DB-DATABASE"),
                    Pooling = true,
                    MinPoolSize = 0,
                    MaxPoolSize = 100,
                    CommandTimeout = 60
                };
                defaultConnection = connectionStringBuilder.ToString();
            }

            return new NpgsqlConnection(defaultConnection);
        }
    }

    public static class DbContextExtensions
    {
        /// <summary>
        /// Executes a query and maps the results to a list of type T using Dapper.
        /// </summary> 
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <typeparam name="U">The type of the parameters to be passed to the query.</typeparam>
        /// <param name="context">The database context.</param>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters to be passed to the query.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of type T.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the context or query is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the parameters
        /// do not match the expected format.</exception>
        public static async Task<IEnumerable<T>> GetAsync<T, U>(this IDbConnection connection, string query, U? parameters)
        {
            List<PropertyInfo> properties = typeof(U).GetProperties().ToList();
            var attributes = properties.SelectMany(p => p.GetCustomAttributes(true).Select(q =>
            {
                return new { CustomAttribute = q, Property = p };

            }))
            .ToList();

            var customAttributes = attributes.Where(x => x.CustomAttribute.GetType().Name == "ColumnMapAttribute")
                .Select(x => new
                {
                    PropertyName = x.Property.Name,
                    AttributeName = x.CustomAttribute.GetType().GetProperty("Name")?.GetValue(x.CustomAttribute, null)?.ToString(),
                    PropertyValue = x.Property.GetValue(parameters, null)
                })
                .ToList();

            var input = new Dictionary<string, object?>();

            // 1. Definimos la fuente de datos para evitar duplicar la lógica de string.Join
            var dataSource = customAttributes.Count > 0
                ? customAttributes.Select(x => new { Name = x.AttributeName ?? x.PropertyName, Value = x.PropertyValue })
                : properties.Select(x => new { Name = x.Name, Value = x.GetValue(parameters) });

            // 2. Llenamos el diccionario de forma explícita (claro y sin efectos secundarios en LINQ)
            foreach (var item in dataSource)
            {
                input.TryAdd(item.Name, item.Value);
            }
            return await connection.QueryAsync<T>(query, input);
        }

        /// <summary>
        /// Executes a query and returns the first result or default value if no results are found.
        /// </summary>
        /// <typeparam name="T">The type to map the result to.</typeparam>
        /// <typeparam name="U">The type of the parameters to be passed to the query.</typeparam>
        /// <param name="context">The database context.</param>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters to be passed to the query.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the first result of type T or null if no results are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the context or query is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the parameters
        /// do not match the expected format.</exception>
        public static async Task<T?> GetFirstOrDefaultAsync<T, U>(this IDbConnection connection, string query, U? parameters)
        {
            if (EqualityComparer<U>.Default.Equals(parameters, default))
                return await connection.QueryFirstOrDefaultAsync<T>(query);

            var dynamicParams = new DynamicParameters();
            var properties = typeof(U).GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(parameters);

                var attr = prop.GetCustomAttributes()
                               .FirstOrDefault(a => a.GetType().Name == "ColumnMapAttribute");

                string paramName = prop.Name;

                if (attr != null)
                {
                    paramName = attr.GetType().GetProperty("Name")?.GetValue(attr)?.ToString() ?? prop.Name;
                }

                dynamicParams.Add(paramName, value);
            }

            return await connection.QueryFirstOrDefaultAsync<T>(query, dynamicParams);
        }
    }
}
