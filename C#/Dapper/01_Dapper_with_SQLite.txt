using System.Data.SQLite;
using Dapper;

var databaseFile = @"D:\Database.sqlite";
var connectionString = $"Data Source={databaseFile};Version=3;";

using var connection = new SQLiteConnection(connectionString);
connection.Open();

// Here you can create your table if it doesn't exist
string createTableQuery = """
                          CREATE TABLE IF NOT EXISTS Logs (
                                Id INTEGER PRIMARY KEY,
                                UserId TEXT NOT NULL,
                                Event TEXT NOT NULL,
                                TimeStamp DATETIME DEFAULT CURRENT_TIMESTAMP
                            );
                          """;

connection.Execute(createTableQuery);