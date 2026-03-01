using Npgsql;

namespace Backend.Api.Infrastructure.Database;

public class PostgresDatabaseHealthService : IDatabaseHealthService
{
    private readonly IConfiguration _configuration;

    public PostgresDatabaseHealthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("Postgres");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Missing connection string to Database");

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var cmd = new NpgsqlCommand("SELECT 1;", connection);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);

            return result is not null;
        }
        catch
        {
            return false;
        }
    }
}
