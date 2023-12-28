using Dapper;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Linq;

namespace SLA_Remake;

public static class Database<T>
{
	// This class manages all the Database related operations.
	// It uses SQLite with Dapper for the Database operations.

	private static class QueryBuilder
	{
		// This class contains all the queries that are used in the Database operations.
		// It uses System.Reflection and Dapper's Query Builder to generate the queries.

		private static readonly Type _type_i = typeof(T);
		private static readonly string _name = _type_i.Name;
		private static readonly System.Reflection.PropertyInfo[] _properties = _type_i.GetProperties();
		private static readonly IEnumerable<string> _header = _properties.Select(p => p.Name);
		private static readonly IEnumerable<string> _values = _properties.Select(p => $"@{p.Name}");

		public static string GenerateTable() => $"CREATE TABLE IF NOT EXISTS {_name} ({string.Join(", ", _header)});";
		public static string InsertEntity() => $"INSERT INTO {_name} ({string.Join(", ", _header)}) VALUES ({string.Join(", ", _values)});";
		public static string RetrieveAll() => $"SELECT * FROM {_name};";
		public static string ClearTable() => $"DELETE FROM {_name};";
	}

	private static readonly string DatabaseLocation = System.IO.Path.Combine(Configuration.MyPath, Configuration.MyName + ".sqlite");
	private static readonly string ConnectionString = $"Data Source={DatabaseLocation};Version=3;";
	private static readonly System.Data.SQLite.SQLiteConnection Connection = new(ConnectionString);

	public static int Save(T entry) => Save([entry]);

	public static int Save(IEnumerable<T> entries)
	{
		if (!Configuration.EnableCacheLogging) return 0;
		lock (Connection)
		{
			Connection.Open();
			var query = QueryBuilder.GenerateTable();
			Connection.Execute(query);

			query = QueryBuilder.InsertEntity();
			var yield = Connection.Execute(query, entries);

			Connection.Close();
			return yield;
		}
	}

	public static List<T> GetSavedEntries()
	{
		if (!Configuration.EnableCacheLogging) return [];
		lock (Connection)
		{
			Connection.Open();
			var query = QueryBuilder.RetrieveAll();
			var yield = Connection.Query<T>(query).ToList();
			Connection.Close();
			return yield;
		}
	}

	public static int Clear()
	{
		if (!Configuration.EnableCacheLogging) return 0;
		lock (Connection)
		{
			Connection.Open();
			var query = QueryBuilder.ClearTable();
			var yield = Connection.Execute(query);
			Connection.Close();
			return yield;
		}
	}
}