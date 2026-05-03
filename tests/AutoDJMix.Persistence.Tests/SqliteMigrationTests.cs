using AutoDJMix.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoDJMix.Persistence.Tests;

public sealed class SqliteMigrationTests
{
    [Fact]
    public void Can_apply_migrations()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AutoDjMixDbContext>()
            .UseSqlite(connection)
            .Options;

        using var db = new AutoDjMixDbContext(options);
        db.Database.Migrate();

        Assert.True(db.Database.CanConnect());
    }
}
