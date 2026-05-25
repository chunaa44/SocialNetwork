using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialPlatformLibrary.Repositories;

namespace SocialPlatformTests;

/// <summary>
/// Opens a shared in-memory SQLite connection for each test class
/// and initialises all tables via DbInitializer.
/// Each test class gets its own connection so tests are fully isolated.
/// </summary>
public abstract class SqliteTestBase
{
    protected SqliteConnection Connection { get; private set; } = null!;

    [TestInitialize]
    public void OpenDb()
    {
        SQLitePCL.Batteries.Init();
        // "Data Source=:memory:" keeps the DB alive for the lifetime of the connection
        Connection = new SqliteConnection("Data Source=:memory:");
        Connection.Open();
        DbInitializer.Initialize(Connection);
    }

    [TestCleanup]
    public void CloseDb()
    {
        Connection.Close();
        Connection.Dispose();
    }
}