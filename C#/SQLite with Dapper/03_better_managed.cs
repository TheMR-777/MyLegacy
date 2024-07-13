using System.Data.SQLite;
using Dapper;

internal class MyProgram
{
    private const string SQLiteStorageLoc = @"D:\Database.sqlite";
    private const string ConnectionString = $"Data Source={SQLiteStorageLoc};Version=3;";
    private static readonly SQLiteConnection connection = new(ConnectionString);

    public static void Main()
    {
        InitializeTable();
    }

    public static void InitializeTable()
    {
        connection.Open();

        const string createTableQuery =
            """
            CREATE TABLE IF NOT EXISTS MyLogs (
                    LogId INTEGER NOT NULL PRIMARY KEY,
                    UserId TEXT,
                    UserName TEXT,
                    UserIp TEXT,
                    LogDate TEXT,
                    LogInTime TEXT,
                    LogOutTime TEXT,
                    LogFlag TEXT,
                    Reason TEXT,
                    ReasonType TEXT,
                    ReasonId TEXT,
                    UserPCName TEXT,
                    UserDisplayName TEXT,
                    LogSide TEXT
                );
            """;

        connection.Execute(createTableQuery);
    }
}