using Dapper;
using System.Collections.Generic;
using System.Linq;
using System;

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
		private static readonly List<string> _header = _properties.Select(p => p.Name).ToList();
		private static readonly List<string> _values = _properties.Select(p => $"@{p.Name}").ToList();

		public static string GenerateTable()
		{
			var query = $"CREATE TABLE IF NOT EXISTS {_name} ({string.Join(", ", _header)});";
			return query;
		}

		public static string Insert()
		{
			var query = $"INSERT INTO {_name} ({string.Join(", ", _header)}) VALUES ({string.Join(", ", _values)});";
			return query;
		}

		public static string SelectAll()
		{
			var query = $"SELECT * FROM {_name};";
			return query;
		}

		public static string ClearTable()
		{
			var query = $"DELETE FROM {_name};";
			return query;
		}
	}

	private const string DatabaseFullName = "Database.sqlite";
	private static readonly string DatabaseLocation = System.IO.Path.Combine(Controls.HomeFolder, DatabaseFullName);
	private static readonly string ConnectionString = $"Data Source={DatabaseLocation};Version=3;";
	private static readonly System.Data.SQLite.SQLiteConnection Connection = new(ConnectionString);

	public static int Save(T entry)
	{
		if (!Controls.EnableCacheLogging) return 0;

		Connection.Open();
		var query = QueryBuilder.GenerateTable();
		Connection.Execute(query);

		query = QueryBuilder.Insert();
		var result = Connection.Execute(query, entry);

		Connection.Close();
		return result;
	}

	public static List<T> GetSavedEntries()
	{
		if (!Controls.EnableCacheLogging) return [];

		Connection.Open();
		var query = QueryBuilder.SelectAll();
		var result = Connection.Query<T>(query).ToList();
		Connection.Close();
		return result;
	}

	public static int Clear()
	{
		if (!Controls.EnableCacheLogging) return 0;

		Connection.Open();
		var query = QueryBuilder.ClearTable();
		var result = Connection.Execute(query);
		Connection.Close();
		return result;
	}
}