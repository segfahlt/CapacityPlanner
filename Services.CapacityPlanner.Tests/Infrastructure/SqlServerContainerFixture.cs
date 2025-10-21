using System.Text;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel;

using Testcontainers.MsSql;

namespace Services.CapacityPlanner.Tests.Infrastructure;

public class SqlServerContainerFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; }
    public string ConnectionString => Container.GetConnectionString();

    public SqlServerContainerFixture()
    {
        Container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("yourStrong(!)Password")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();

        // Create database and apply schema from Tables.build.sql if present
        var dbName = "CapacityPlannerTest";
        var master = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = "master" }.ToString();
        await using var conn = new SqlConnection(master);
        await conn.OpenAsync();
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}];";
            await cmd.ExecuteNonQueryAsync();
        }

        var dbCnn = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = dbName }.ToString();

        // Try load build script from repo
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var buildScriptPath = Path.Combine(repoRoot, "Persist.CapacityPlanner", "RawDbFiles", "Tables", "Tables.build.sql");
        if (File.Exists(buildScriptPath))
        {
            var sql = await File.ReadAllTextAsync(buildScriptPath, Encoding.UTF8);
            await using var dbConn = new SqlConnection(dbCnn);
            await dbConn.OpenAsync();
            // Split on GO
            var batches = sql.Split(new[]{"\r\nGO\r\n", "\nGO\n", "\r\nGO\n", "\nGO\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var batch in batches)
            {
                await using var c = dbConn.CreateCommand();
                c.CommandTimeout = 120;
                c.CommandText = batch;
                await c.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }

    public DbContextOptions<CapacityPlannerContext> CreateOptions()
    {
        var cnn = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = "CapacityPlannerTest" }.ToString();
        var builder = new DbContextOptionsBuilder<CapacityPlannerContext>();
        builder.UseSqlServer(cnn);
        return builder.Options;
    }
}

// Collection definition so the container is shared across tests
[CollectionDefinition("sql-server")] 
public class SqlServerCollection : ICollectionFixture<SqlServerContainerFixture> { }
