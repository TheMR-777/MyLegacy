using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SLA_Remake;

public static class Database<T>
{
    // This class manages all the Database related operations.
    // It uses SQLite with Dapper for the Database operations.

    private static class QueryBuilder
    {
        // This class contains all the queries that are used in the Database operations.
        // It uses System.Reflection and Dapper's Query Builder to generate the queries.

        private static readonly System.Type _type_i = typeof(T);
        private static readonly string _name = _type_i.Name;
        private static readonly System.Reflection.PropertyInfo[] _properties = _type_i.GetProperties();
        private static readonly IEnumerable<string> _header = _properties.Select(p => p.Name);
        private static readonly IEnumerable<string> _values = _properties.Select(p => $"@{p.Name}");

        public static string GenerateTable() => $"CREATE TABLE IF NOT EXISTS {_name} ({string.Join(", ", _header)});";
        public static string InsertEntity() => $"INSERT INTO {_name} ({string.Join(", ", _header)}) VALUES ({string.Join(", ", _values)});";
        public static string RetrieveAll() => $"SELECT * FROM {_name};";
        public static string ClearTable() => $"DELETE FROM {_name};";
    }

    public static readonly string MyName = System.AppDomain.CurrentDomain.FriendlyName;
    public static readonly string MyPath = System.AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string DatabaseLocation = System.IO.Path.Combine(MyPath, MyName + ".sqlite");
    private static readonly string ConnectionString = $"Data Source={DatabaseLocation};Version=3;";
    private static readonly object UltimateLockdown = new();
    private static System.Data.SQLite.SQLiteConnection GetConnection => new(ConnectionString);

    public static int Save(T entry) => Save([entry]);

    public static int Save(IEnumerable<T> entries)
    {
        lock (UltimateLockdown)
        {
            using var Connection = GetConnection;
            var query = QueryBuilder.GenerateTable();
            Connection.Execute(query);

            query = QueryBuilder.InsertEntity();
            var yield = Connection.Execute(query, entries);

            return yield;
        }
    }

    public static List<T> GetSavedEntries()
    {
        lock (UltimateLockdown)
        {
            using var Connection = GetConnection;
            var query = QueryBuilder.GenerateTable();
            Connection.Execute(query);

            query = QueryBuilder.RetrieveAll();
            var yield = Connection.Query<T>(query).ToList();

            return yield;
        }
    }

    public static int Clear()
    {
        lock (UltimateLockdown)
        {
            using var Connection = GetConnection;
            var query = QueryBuilder.GenerateTable();
            Connection.Execute(query);

            query = QueryBuilder.ClearTable();
            var yield = Connection.Execute(query);

            return yield;
        }
    }
}

public static class HeavyTest
{
    private const int threads_n = 1000;

    private class TestEntity
    {
        public TestEntity() { }

        public TestEntity(int id)
        {
            Id = id;
            Name = $"Entity {id}";
        }

        public int Id { get; set; } = default;
        public string Name { get; set; } = string.Empty;
    }

    private static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Starting tests...");

            TestConcurrentWrites();
            TestConcurrentReads();
            TestConcurrentReadWrites();

            Console.WriteLine("Tests completed.");
        }

        private static void ClearDatabase()
        {
            Database<TestEntity>.Clear();
            Console.WriteLine();
            Console.WriteLine("Database cleared.");
        }

        private static void TestConcurrentWrites()
        {
            ClearDatabase();
            Console.WriteLine("Testing concurrent writes...");

            var tasks = new List<Task>();

            for (var i = 0; i < threads_n; i++)
            {
                TestEntity entity = new(i);
                tasks.Add(Task.Run(() => Database<TestEntity>.Save(entity)));
            }

            Task.WhenAll(tasks).Wait();

            var savedEntries = Database<TestEntity>.GetSavedEntries();
            Console.WriteLine($"Expected entries: {threads_n}, Actual entries: {savedEntries.Count}");
        }

        private static void TestConcurrentReads()
        {
            ClearDatabase();
            Console.WriteLine("Testing concurrent reads...");

            var entities = Enumerable.Range(0, threads_n).Select(i => new TestEntity(i)).ToList();
            Database<TestEntity>.Save(entities);

            var tasks = new List<Task<List<TestEntity>>>();

            for (var i = 0; i < threads_n; i++)
            {
                tasks.Add(Task.Run(Database<TestEntity>.GetSavedEntries));
            }

            var results = Task.WhenAll(tasks).Result;

            var allReadCorrectly = results.All(result => result.Count == threads_n);
            Console.WriteLine($"All reads returned {threads_n} entries: {allReadCorrectly}");
        }

        private static void TestConcurrentReadWrites()
        {
            ClearDatabase();
            Console.WriteLine("Testing concurrent reads and writes...");

            const int n = threads_n / 2;

            var writeTasks = new List<Task>();
            var readTasks = new List<Task<List<TestEntity>>>();

            for (var i = 0; i < n; i++)
            {
                var entity = new TestEntity(i);
                writeTasks.Add(Task.Run(() => Database<TestEntity>.Save(entity)));
            }

            for (var i = 0; i < n; i++)
            {
                readTasks.Add(Task.Run(Database<TestEntity>.GetSavedEntries));
            }

            Task.WhenAll(writeTasks).Wait();
            var readResults = Task.WhenAll(readTasks).Result;

            var readCountsCorrect = readResults.All(result => result.Count <= n);
            Console.WriteLine($"All read operations returned {n} or fewer entries: {readCountsCorrect}");
        }
    }
}
