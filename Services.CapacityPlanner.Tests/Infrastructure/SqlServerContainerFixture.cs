using System.Text;

using Common.CapacityPlanner;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;

namespace Services.CapacityPlanner.Tests.Infrastructure;

public class SqlServerContainerFixture : IAsyncLifetime
{
    public string ServerConnectionString { get; private set; } = string.Empty;
    public string DatabaseConnectionString { get; private set; } = string.Empty;

    public SqlServerContainerFixture() { }

    public async Task InitializeAsync()
    {
        // Determine server connection string
        // Prefer env var CAPACITYPLANNER_TEST_SQL, else LocalDB
        var baseCnn = Environment.GetEnvironmentVariable("CAPACITYPLANNER_TEST_SQL");
        if (string.IsNullOrWhiteSpace(baseCnn))
        {
            // Default LocalDB
            var sb = new SqlConnectionStringBuilder
            {
                DataSource = @".",
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true
            };
            baseCnn = sb.ToString();
        }

        // Create database and apply schema from Tables.build.sql if present
        var dbName = $"CapacityPlannerTest_{Guid.NewGuid():N}";
        var master = new SqlConnectionStringBuilder(baseCnn) { InitialCatalog = "CP" }.ToString();
        await using var conn = new SqlConnection(master);
        await conn.OpenAsync();
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}];";
            await cmd.ExecuteNonQueryAsync();
        }

        ServerConnectionString = baseCnn;
        DatabaseConnectionString = new SqlConnectionStringBuilder(baseCnn) { InitialCatalog = dbName }.ToString();

        // Try load build script from repo
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var buildScriptPath = Path.Combine(repoRoot, "Persist.CapacityPlanner", "RawDbFiles", "Tables", "Tables.build.sql");
        if (File.Exists(buildScriptPath))
        {
            var sql = await File.ReadAllTextAsync(buildScriptPath, Encoding.UTF8);
            await using var dbConn = new SqlConnection(DatabaseConnectionString);
            await dbConn.OpenAsync();
            await using var c = dbConn.CreateCommand();
            c.CommandTimeout = 120;
            // Split on GO
            var batches = sql.Split(new[] { "GO\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var batch in batches)
            {
                c.CommandText = batch.Trim();
                if(string.IsNullOrWhiteSpace(c.CommandText)) continue;
				await c.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task DisposeAsync()
    {
        if (!string.IsNullOrWhiteSpace(DatabaseConnectionString))
        {
            try
            {
                var dbName = new SqlConnectionStringBuilder(DatabaseConnectionString).InitialCatalog;
                var master = new SqlConnectionStringBuilder(ServerConnectionString) { InitialCatalog = "master" }.ToString();
                await using var conn = new SqlConnection(master);
                await conn.OpenAsync();
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = $"IF DB_ID('{dbName}') IS NOT NULL BEGIN ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{dbName}]; END";
                await cmd.ExecuteNonQueryAsync();
            }
            catch { /* best effort */ }
        }
    }

    public DbContextOptions<CapacityPlannerContext> CreateOptions()
    {
        var builder = new DbContextOptionsBuilder<CapacityPlannerContext>();
        builder.UseSqlServer(DatabaseConnectionString);
        return builder.Options;
    }
}

// Collection definition so the container is shared across tests
[CollectionDefinition("sql-server")] 
public class SqlServerCollection : ICollectionFixture<SqlServerContainerFixture> { }
