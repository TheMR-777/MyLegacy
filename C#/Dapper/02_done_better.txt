using System.Data.SQLite;
using Dapper;

const string databaseFile = @"D:\Database.sqlite";
const string connectionString = $"Data Source={databaseFile};Version=3;";

using var connection = new SQLiteConnection(connectionString);
connection.Open();

const string createTableQuery = 
"""
CREATE TABLE IF NOT EXISTS MyLogs (
        Id INTEGER PRIMARY KEY,
        UserId TEXT NOT NULL,
        Event TEXT NOT NULL,
        TimeStamp DATETIME DEFAULT CURRENT_TIMESTAMP
    );
""";

connection.Execute(createTableQuery);